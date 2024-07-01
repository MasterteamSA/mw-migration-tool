using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MWDataMigrationApp.Data;
using MWDataMigrationApp.Data.SourceModels;
using MWDataMigrationApp.Data.TargetModels;
using MWDataMigrationApp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MWDataMigrationApp
{
    public class DataMigrationService
    {
        private readonly SourceDbContext _sourceContext;
        private readonly TargetContext _targetContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DataMigrationService(SourceDbContext sourceContext, TargetContext targetContext, IConfiguration configuration, HttpClient httpClient)
        {
            _sourceContext = sourceContext;
            _targetContext = targetContext;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async System.Threading.Tasks.Task MigrateData()
        {

            string token = await AuthenticateAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var preparedUsers = FetchAndPrepareUsers();


            var seededUsers = await GetSeededUsersAsync(); 


            var usersToCreate = preparedUsers.Where(user => !seededUsers.ContainsKey(user.AD_UserPrincipalName.Trim())).ToList();

            List<UserInfo> createdUsersInfo = new();
            if (usersToCreate.Any())
            {
                createdUsersInfo = await CreateUsersAsync(usersToCreate);
            }

            var allRelevantUsers = seededUsers.Values.ToList();
            allRelevantUsers.AddRange(createdUsersInfo);


            var userAccounts = _sourceContext.UsersMts.ToList();

            var accountIdToEmailMap = userAccounts.ToDictionary(u => u.UserAccountId, u => u.Email);

            var projectRoleMembersMts = _sourceContext.ProjectRoleMembersMts.ToList();

            var domainMts = _sourceContext.DomainMts.ToList();
            var domainMapping = await GetDomainMappingsAsync();

            //string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domains_log.txt");
            //using (var logWriter = new StreamWriter(logFilePath, append: true))
            //{
            //    foreach (var domainMt in domainMts)
            //    {
            //        int domainId = await CreateDomainAsync(domainMt, accountIdToEmailMap, allRelevantUsers, logWriter);
            //        if (domainId != 0)
            //        {
            //            domainMapping[domainMt.DomainName] = domainId;
            //        }
            //    }
            //}

            var mtProjects = _sourceContext.ProjectsMts.ToList();
            var exclud = new List<long> { 10030, 10022 };

            foreach (var mtProject in mtProjects.Where(x => !exclud.Contains(x.ProjectId.Value)))
            {
                var projectDomains = GetDomainsByProject(mtProject.ProjectId.Value);
                var domainIds = projectDomains.Select(d => domainMapping[d.DomainName]).Distinct().ToList();

                var result = await CreateProjectAsync(mtProject, domainIds, userAccounts, allRelevantUsers, projectRoleMembersMts);

                 LogProjectResult(result);

                int projectId = result.ProjectId;

                await System.Threading.Tasks.Task.Delay(20000);

                long mtProjectId = mtProject.ProjectId.Value;

                await PostTaskAsync(mtProjectId, projectId, mtProject.StartDate.Value, mtProject.EndDate.Value, accountIdToEmailMap, allRelevantUsers);
                await PostMilestoneAsync(mtProjectId, projectId, mtProject.StartDate.Value, mtProject.EndDate.Value, accountIdToEmailMap, allRelevantUsers);
                var  deliverables = await PostDeliverableAsync(mtProjectId, projectId, mtProject.StartDate.Value, mtProject.EndDate.Value, accountIdToEmailMap, allRelevantUsers);

                await ProcessDeliverablesAsync(projectId, deliverables, token);

                await PostRiskAsync(mtProjectId, projectId, deliverables, mtProject.EndDate.Value, accountIdToEmailMap, allRelevantUsers);
                await PostIssueAsync(mtProjectId, projectId, new List<DeliverableInfo>(), mtProject.EndDate.Value, accountIdToEmailMap, allRelevantUsers);

                var projectExpenses = _sourceContext.TempoExpensesMts.Where(e => e.ProjectId == mtProjectId).ToList();
                await CreateExpensesAsync(projectId, projectExpenses);
            }
        }

        private async Task<string> AuthenticateAsync()
        {
            var username = _configuration["ApiCredentials:Username"];
            var password = _configuration["ApiCredentials:Password"];
            var authUri = _configuration["ApiEndpoints:Authenticate"];

            var requestData = new
            {
                username = username,
                password = password,
                IsEncrypted = true
            };

            var response = await _httpClient.PostAsJsonAsync(authUri, requestData);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseData);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.TryGetProperty("token", out JsonElement tokenElement))
                {
                    return tokenElement.GetString();
                }
            }

            throw new Exception("Authentication failed.");
        }


        public async Task<Dictionary<string, int>> GetDomainMappingsAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/charts/Levels/342/cards");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<JObject>(responseBody);

            var domainMapping = new Dictionary<string, int>();

            var levels = responseJson["data"]["details"][0]["data"];
            foreach (var level in levels)
            {
                var id = level["id"].Value<int>();
                var name = level["name"].Value<string>();
                domainMapping[name] = id;
            }

            return domainMapping;
        }

        public List<AdJiraUser> FetchAndPrepareUsers()
        {
            var rawUsers = _sourceContext.AdJiraUsers.ToList();
            var preparedUsers = rawUsers.Select(user => new AdJiraUser
            {
                AD_DisplayName = string.IsNullOrEmpty(user.AD_DisplayName) ? user.JIRA_NAME : user.AD_DisplayName,
                AD_UserPrincipalName = string.IsNullOrEmpty(user.AD_UserPrincipalName) ? user.JIRA_EMAIL : user.AD_UserPrincipalName,
                AD_USER_NAME = string.IsNullOrEmpty(user.AD_USER_NAME) ? GetUsernameFromEmail(user.JIRA_EMAIL) : GetUsernameFromEmail(user.AD_UserPrincipalName),
                JIRA_EMAIL = user.JIRA_EMAIL,
                JIRA_NAME = user.JIRA_NAME,
                JIRA_USER_NAME = GetUsernameFromEmail(user.JIRA_EMAIL)
            }).ToList();

            return preparedUsers.DistinctBy(x => x.AD_UserPrincipalName).ToList();
        }

        private string GetUsernameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            var atIndex = email.IndexOf('@');
            return atIndex > 0 ? email.Substring(0, atIndex) : null;
        }

        private async Task<Dictionary<string, UserInfo>> GetSeededUsersAsync()
        {
            var users = new Dictionary<string, UserInfo>();
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/identity/Users?Query=");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<UserInfo>>(content, options);
                if (apiResponse?.Data != null)
                {
                    foreach (var userInfo in apiResponse.Data)
                    {
                        users[userInfo.UserName.Trim()] = userInfo;
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to fetch seeded users.");
            }
            return users;
        }
        private async Task<List<UserInfo>> CreateUsersAsync(List<AdJiraUser> usersToCreate)
        {
            var createdUsers = new List<UserInfo>();
            var successLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "successful_user_passwords.txt");
            var failedLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "failed_users.txt");

            using (StreamWriter successWriter = new StreamWriter(successLogFilePath, true))
            using (StreamWriter failedWriter = new StreamWriter(failedLogFilePath, true))
            {
                foreach (var user in usersToCreate)
                {
                    var randomPassword = GenerateRandomPassword();

                    var jsonData = new
                    {
                        userName = user.AD_UserPrincipalName,
                        displayName = user.AD_DisplayName,
                        email = user.AD_UserPrincipalName,
                        mobile = "9977997797",
                        ext = "1",
                        password = randomPassword,
                        isExternal = true,
                        hourRate = 0,
                        IsEncrypted = false
                    };

                    var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/identity/user", jsonData);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        failedWriter.WriteLine($"{DateTime.Now}: Failed to create user {user.AD_UserPrincipalName} - {errorContent}");
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var userInfo = System.Text.Json.JsonSerializer.Deserialize<UserInfo>(JsonDocument.Parse(responseContent).RootElement.GetProperty("data").GetRawText());
                        createdUsers.Add(userInfo);
                        successWriter.WriteLine($"{DateTime.Now}: Email: {user.AD_UserPrincipalName} => Password: {randomPassword}");
                    }
                    await System.Threading.Tasks.Task.Delay(2000);
                }
            }

            return createdUsers;
        }



        private string GenerateRandomPassword(int length = 12)
        {
            if (length < 6)
                throw new ArgumentException("Password length must be at least 6", nameof(length));

            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "1234567890";
            const string specialChars = "!@#$%^&*()";
            const string validChars = upperChars + lowerChars + digitChars + specialChars;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            password.Append(upperChars[random.Next(upperChars.Length)]);
            password.Append(lowerChars[random.Next(lowerChars.Length)]);
            password.Append(digitChars[random.Next(digitChars.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            for (int i = 4; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }


        private async Task<int> CreateDomainAsync(DomainMt domainMt, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users, StreamWriter logWriter)
        {
            try
            {
                var domainUri = _configuration["ApiEndpoints:PostProgram"];
                var propsConfig = _configuration.GetSection("ProgramProperties");

                var defaultUserId = _configuration.GetValue<string>("DefaultUserId");
                var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

                var assignedToEmail = domainMt.UserAccountId != null && accountIdToEmailMap.ContainsKey(domainMt.UserAccountId)
                       ? accountIdToEmailMap[domainMt.UserAccountId]
                       : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    await logWriter.WriteLineAsync($"Default user assigned for Domain: {domainMt.DomainName}");
                }

                var requestBody = new
                {
                    name = new { ar = domainMt.DomainName, en = domainMt.DomainName },
                    props = new List<object>
                {
                    CreateProp(propsConfig.GetSection("Manager"), assignedTo),
                    CreateProp(propsConfig.GetSection("Description"), new { ar = $"<p>{domainMt.DomainName}</p>", en = $"<p>{domainMt.DomainName}</p>" })
                },
                    levelId = _configuration.GetValue<int>("ProgramLevelId"),
                    sources = new List<int> { _configuration.GetValue<int>("ProgramSources") },
                    attachments = new List<object>(),
                    isDraft = false
                };

                var response = await _httpClient.PostAsJsonAsync(domainUri, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseData);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        await logWriter.WriteLineAsync($"Success: Domain {domainMt.DomainName} created with ID {dataElement.GetInt32()}");
                        return dataElement.GetInt32();
                    }
                }

                var errorResponse = await response.Content.ReadAsStringAsync();
                await logWriter.WriteLineAsync($"Failed: Domain {domainMt.DomainName} creation failed. Error: {errorResponse}");
                return 0;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        private IEnumerable<DomainMt> GetDomainsByProject(long projectId)
        {
            var domainNames = _sourceContext.ProjectDomainMts
                .Where(pd => pd.ProjectId == projectId)
                .Select(pd => pd.DomainName)
                .Distinct()
                .ToList();

            return _sourceContext.DomainMts
                .Where(d => domainNames.Contains(d.DomainName))
                .ToList();
        }

        private async Task<ProjectCreationResult> CreateProjectAsync(ProjectsMt project, List<int> domainIds, List<UsersMt> userAccounts, List<UserInfo> users, List<ProjectRoleMembersMt> projectRoleMembersMts)
        {
            var postUri = _configuration["ApiEndpoints:PostProject"];
            var propsConfig = _configuration.GetSection("ProjectProperties");

            var mappedBusinessLine = BusinessLineMapping.ContainsKey(project.BusninessLine.TrimEnd()) ? BusinessLineMapping[project.BusninessLine.TrimEnd()] : null;
            var mappedCompany = CompanyMapping.ContainsKey(project.Company.TrimEnd()) ? CompanyMapping[project.Company.TrimEnd()] : null;

            project.Description = string.IsNullOrEmpty(project.Description) ? "Default Description" : project.Description;

            var startDate = project.StartDate.HasValue ? project.StartDate.Value.ToString("yyyy-MM-dd") : "2023-01-01"; 
            var endDate = project.EndDate.HasValue ? project.EndDate.Value.ToString("yyyy-MM-dd") : "2024-12-28"; 

            var managerUserId = GetUserIdByRole(project.ProjectId.Value, "Project Manager", projectRoleMembersMts, userAccounts, users);
            var programDirectorUserId = GetUserIdByRole(project.ProjectId.Value, "Program Director", projectRoleMembersMts, userAccounts, users);
            var accountManagerUserId = GetUserIdByEmail(project.AccountManager, userAccounts, users);

            bool defaultManagerAdded = managerUserId == null;
            bool defaultProgramDirectorAdded = programDirectorUserId == null;
            bool defaultAccountManagerAdded = accountManagerUserId == null;

            var requestBody = new
            {
                name = new { ar = project.Name, en = project.Name },
                props = new List<object>
            {
                CreateProp(propsConfig.GetSection("StartDate"), startDate),
                CreateProp(propsConfig.GetSection("FinishDate"), endDate),
                CreateProp(propsConfig.GetSection("Budget"), project.ProjectContractValue.HasValue && project.ProjectContractValue > 0 ? project.ProjectContractValue.Value : "1"),
                CreateProp(propsConfig.GetSection("Manager"), managerUserId ?? _configuration.GetValue<string>("DefaultUserId")),
                CreateProp(propsConfig.GetSection("ProgramDirector"), programDirectorUserId ?? _configuration.GetValue<string>("DefaultUserId")),
                CreateProp(propsConfig.GetSection("ClientName"), project.Organization),
                CreateProp(propsConfig.GetSection("AccountManager"), accountManagerUserId),
                CreateProp(propsConfig.GetSection("ProjectServices"), "Support"),
                CreateProp(propsConfig.GetSection("BusninessLine"), mappedBusinessLine),
                CreateProp(propsConfig.GetSection("Company"), mappedCompany),
                CreateProp(propsConfig.GetSection("PipeDriveLink"), project.PipeDriveLink),
                CreateProp(propsConfig.GetSection("Description"), new { ar = $"<p>{project.Description}</p>", en = $"<p>{project.Description}</p>" })
            },
                levelId = _configuration.GetValue<int>("ProjectLevelId"),
                sources = domainIds,
                attachments = new List<object>(),
                isDraft = false
            };

            var response = await _httpClient.PostAsJsonAsync(postUri, requestBody);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("data", out JsonElement dataElement))
                {
                    return new ProjectCreationResult
                    {
                        ProjectId = dataElement.GetInt32(),
                        DefaultStartDateAdded = !project.StartDate.HasValue,
                        DefaultEndDateAdded = !project.EndDate.HasValue,
                        DefaultManagerAdded = defaultManagerAdded,
                        DefaultProgramDirectorAdded = defaultProgramDirectorAdded,
                        DefaultAccountManagerAdded = defaultAccountManagerAdded
                    };
                }
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                // Handle error response
            }
            return new ProjectCreationResult
            {
                ProjectId = 0,
                DefaultStartDateAdded = !project.StartDate.HasValue,
                DefaultEndDateAdded = !project.EndDate.HasValue,
                DefaultManagerAdded = defaultManagerAdded,
                DefaultProgramDirectorAdded = defaultProgramDirectorAdded,
                DefaultAccountManagerAdded = defaultAccountManagerAdded
            };
        }
        private string GetUserIdByRole(long projectId, string roleName, List<ProjectRoleMembersMt> projectRoleMembersMts, List<UsersMt> userAccounts, List<UserInfo> users)
        {
            var accountUserId = projectRoleMembersMts.FirstOrDefault(rm => rm.ProjectId == projectId && rm.ProjectRoleName == roleName)?.UserAccountId;
            if (accountUserId != null)
            {
                var email = userAccounts.FirstOrDefault(ua => ua.UserAccountId == accountUserId)?.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    var userId = users.FirstOrDefault(u => u.Email == email)?.UserId;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        return userId;
                    }
                }
            }

            var fallbackAccountUserId = projectRoleMembersMts.FirstOrDefault(rm => rm.ProjectId == projectId && rm.ProjectRoleName == "Programs Managers")?.UserAccountId;
            if (fallbackAccountUserId != null)
            {
                var fallbackEmail = userAccounts.FirstOrDefault(ua => ua.UserAccountId == fallbackAccountUserId)?.Email;
                if (!string.IsNullOrEmpty(fallbackEmail))
                {
                    return users.FirstOrDefault(u => u.Email == fallbackEmail)?.UserId;
                }
            }
            return null;
        }

        private string GetUserIdByEmail(string userName, List<UsersMt> userAccounts, List<UserInfo> users)
        {
            var email = userAccounts.FirstOrDefault(ua => ua.UserName == userName)?.Email;
            if (!string.IsNullOrEmpty(email))
            {
                return users.FirstOrDefault(u => u.Email == email)?.UserId;
            }
            return null;
        }

        private void LogProjectResult(ProjectCreationResult result)
        {
            var logMessage = $"Project ID: {result.ProjectId}, Success: {result.ProjectId > 0}, " +
                             $"Default Start Date Added: {result.DefaultStartDateAdded}, Default End Date Added: {result.DefaultEndDateAdded}, " +
                             $"Default Manager Added: {result.DefaultManagerAdded}, Default Program Director Added: {result.DefaultProgramDirectorAdded}, " +
                             $"Default Account Manager Added: {result.DefaultAccountManagerAdded}";
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "project_log.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {logMessage}");
            }
        }

        private object CreateProp(IConfigurationSection propConfig, object value)
        {
            return new
            {
                id = 0,
                propertyId = propConfig.GetValue<int>("Id"),
                key = propConfig.GetValue<string>("Key"),
                value = value,
                viewType = propConfig.GetValue<string>("ViewType"),
            };
        }

        private async System.Threading.Tasks.Task PostTaskAsync(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate,
     Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            var taskTypeIds = new List<long> { 10007, 10008, 10064, 10111, 10113, 10114, 10117 };
            var tasks = _sourceContext.IssuesMts.Where(x => taskTypeIds.Contains(x.IssueTypeId.Value) && x.ProjectId == mtProjectId).Select(i => new
            {
                i.IssueId,
                Name = i.Summary,
                Details = i.Description,
                ActualStartDate = i.ActualStartDate,
                PlannedStartDate = i.PlannedStartDate,
                ActualEndDate = i.ActualEndDate,
                PlannedEndDate = i.PlannedEndDate,
                AssignedToAccountId = i.CurrentAssigneeAccountId,
                Type = i.Type10074,
                i.IssueStatusName,
                ParentIssueId = i.ParentIssueId
            }).ToList();

            var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

            var taskPropsSection = _configuration.GetSection("TaskProperties:Type");

            int propertyId = taskPropsSection.GetValue<int>("PropertyId");
            string key = taskPropsSection.GetValue<string>("Key");
            string viewType = taskPropsSection.GetValue<string>("ViewType");

            var parentTasks = tasks.Where(t => t.ParentIssueId == null).ToList();
            var childTasks = tasks.Where(t => t.ParentIssueId != null).ToList();
            var parentTaskMapping = new Dictionary<long, string>();

            var defaultUserId = _configuration.GetValue<string>("DefaultUserId");

            // Load existing records from files
            var successRecords = new List<string>(await LoadRecordsAsync("tasksSuccessRecords.txt"));
            var failedRecords = new List<string>(await LoadRecordsAsync("tasksFailedRecords.txt"));
            var dateFallbackRecords = new List<string>(await LoadRecordsAsync("tasksDateFallbackRecords.txt"));

            // create parent tasks
            foreach (var parentTask in parentTasks)
            {
                var startDate = parentTask.ActualStartDate ?? parentTask.PlannedStartDate ?? projectStartDate;
                var finishDate = parentTask.ActualEndDate ?? parentTask.PlannedEndDate ?? projectFinishDate;

                if (parentTask.ActualStartDate == null || parentTask.ActualEndDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Parent Task: {parentTask.Name}, StartDate: {startDate}, FinishDate: {finishDate}");
                }

                var mappedType = parentTask.Type != null && TypeMapping.ContainsKey(parentTask.Type.TrimEnd()) ? TypeMapping[parentTask.Type.TrimEnd()] : null;

                var progress = StatusProgressMapping.ContainsKey(parentTask.IssueStatusName.TrimEnd()) ? StatusProgressMapping[parentTask.IssueStatusName.TrimEnd()] : 0;

                var assignedToEmail = parentTask.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(parentTask.AssignedToAccountId)
                    ? accountIdToEmailMap[parentTask.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    failedRecords.Add($"Project ID: {projectId}, Parent Task: {parentTask.Name}");
                }

                var requestBody = new
                {
                    name = parentTask.Name,
                    details = parentTask.Details,
                    assignedTo,
                    startDate = startDate.ToString("yyyy-MM-dd"),
                    finishDate = finishDate.ToString("yyyy-MM-dd"),
                    attachments = new List<object>(),
                    status = "",
                    progress,
                    props = new List<object>
            {
                new
                {
                    id = 0,
                    propertyId,
                    key,
                    value = mappedType,
                    viewType
                }
            },
                    type = "Task"
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Tasks", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonDocumentOptions
                    {
                        CommentHandling = JsonCommentHandling.Skip
                    };
                    using (var document = JsonDocument.Parse(responseContent, options))
                    {
                        var data = document.RootElement.GetProperty("data");
                        parentTaskMapping[parentTask.IssueId.Value] = data.GetProperty("guid").GetString();

                        successRecords.Add($"Project ID: {projectId}, Parent Task: {parentTask.Name}");
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    failedRecords.Add($"Project ID: {projectId}, Parent Task: {parentTask.Name}, IssueId: {parentTask.IssueId}, Error: {errorResponse}");
                }
                await System.Threading.Tasks.Task.Delay(3000);
            }

            // create child tasks
            foreach (var childTask in childTasks)
            {
                var startDate = childTask.ActualStartDate ?? childTask.PlannedStartDate ?? projectStartDate;
                var finishDate = childTask.ActualEndDate ?? childTask.PlannedEndDate ?? projectFinishDate;

                if (childTask.ActualStartDate == null || childTask.ActualEndDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Child Task: {childTask.Name}, StartDate: {startDate}, FinishDate: {finishDate}");
                }

                var mappedType = childTask.Type != null && TypeMapping.ContainsKey(childTask.Type.TrimEnd()) ? TypeMapping[childTask.Type.TrimEnd()] : null;

                var progress = StatusProgressMapping.ContainsKey(childTask.IssueStatusName) ? StatusProgressMapping[childTask.IssueStatusName] : 0;

                var assignedToEmail = childTask.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(childTask.AssignedToAccountId)
                    ? accountIdToEmailMap[childTask.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    failedRecords.Add($"Project ID: {projectId}, Child Task: {childTask.Name}");
                }

                var requestBody = new
                {
                    name = childTask.Name,
                    details = childTask.Details,
                    assignedTo,
                    startDate = startDate.ToString("yyyy-MM-dd"),
                    finishDate = finishDate.ToString("yyyy-MM-dd"),
                    attachments = new List<object>(),
                    status = "",
                    progress,
                    props = new List<object>
            {
                new
                {
                    id = 0,
                    propertyId,
                    key,
                    value = mappedType,
                    viewType
                }
            },
                    type = "Task",
                    parentGuid = parentTaskMapping.ContainsKey(childTask.ParentIssueId.Value) ? parentTaskMapping[childTask.ParentIssueId.Value] : null
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Tasks", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    successRecords.Add($"Project ID: {projectId}, Child Task: {childTask.Name}");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    failedRecords.Add($"Project ID: {projectId}, Child Task: {childTask.Name},  IssueId: {childTask.IssueId}, Error: {errorResponse}");
                }
                await System.Threading.Tasks.Task.Delay(3000);
            }

            await SaveRecordsAsync("tasksSuccessRecords.txt", successRecords);
            await SaveRecordsAsync("tasksFailedRecords.txt", failedRecords);
            await SaveRecordsAsync("tasksDateFallbackRecords.txt", dateFallbackRecords);
        }

        private async Task<IEnumerable<string>> LoadRecordsAsync(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return await System.IO.File.ReadAllLinesAsync(filePath);
            }
            return Enumerable.Empty<string>();
        }

        private async System.Threading.Tasks.Task SaveRecordsAsync(string filePath, IEnumerable<string> records)
        {
            await System.IO.File.WriteAllLinesAsync(filePath, records);
        }

        private async System.Threading.Tasks.Task PostMilestoneAsync(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate,
    Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            var milestones = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10006 && x.ProjectId == mtProjectId).Select(i => new
            {
                i.IssueId,
                Name = i.Summary,
                Details = i.Description,
                ActualStartDate = i.ActualStartDate,
                PlannedStartDate = i.PlannedStartDate,
                ActualEndDate = i.ActualEndDate,
                PlannedEndDate = i.PlannedEndDate,
                AssignedToAccountId = i.CurrentAssigneeAccountId,
                Type = i.Type10074,
                i.IssueStatusName,
                Status = ""
            }).ToList();

            var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

            var milestonePropsSection = _configuration.GetSection("MilestoneProperties:Type");

            int propertyId = milestonePropsSection.GetValue<int>("PropertyId");
            string key = milestonePropsSection.GetValue<string>("Key");
            string viewType = milestonePropsSection.GetValue<string>("ViewType");

            var defaultUserId = _configuration.GetValue<string>("DefaultUserId");

            // Load existing records from files
            var successRecords = new List<string>(await LoadRecordsAsync("milestonesSuccessRecords.txt"));
            var failedRecords = new List<string>(await LoadRecordsAsync("milestonesFailedRecords.txt"));
            var dateFallbackRecords = new List<string>(await LoadRecordsAsync("milestonesDateFallbackRecords.txt"));

            foreach (var milestone in milestones)
            {
                var startDate = milestone.ActualStartDate ?? milestone.PlannedStartDate ?? projectStartDate;
                var finishDate = milestone.ActualEndDate ?? milestone.PlannedEndDate ?? projectFinishDate;

                if (milestone.ActualStartDate == null || milestone.ActualEndDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, StartDate: {startDate}, FinishDate: {finishDate}");
                }

                var mappedType = milestone.Type != null && TypeMapping.ContainsKey(milestone.Type.TrimEnd()) ? TypeMapping[milestone.Type.TrimEnd()] : null;

                var progress = StatusProgressMapping.ContainsKey(milestone.IssueStatusName.TrimEnd()) ? StatusProgressMapping[milestone.IssueStatusName.TrimEnd()] : 0;

                var assignedToEmail = milestone.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(milestone.AssignedToAccountId)
                    ? accountIdToEmailMap[milestone.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, AssignedTo: Added default user.");
                }

                var requestBody = new
                {
                    name = milestone.Name,
                    plannedStartDate = startDate.ToString("yyyy-MM-dd"),
                    plannedFinishDate = finishDate.ToString("yyyy-MM-dd"),
                    weight = 0.00001,
                    status = milestone.Status,
                    attachments = new List<object>(),
                    props = new List<object>
            {
                new
                {
                    id = 0,
                    propertyId,
                    key,
                    value = mappedType,
                    viewType,
                }
            }
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Milestones", requestBody);

                int retryCount = 3;
                int currentRetry = 0;

                while (currentRetry < retryCount)
                {
                    try
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            failedRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, IssueId: {milestone.IssueId}, Error: {errorResponse}");
                            break;
                        }
                        else
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var data = JsonDocument.Parse(responseContent).RootElement.GetProperty("data");
                            var taskId = data.GetProperty("taskId").GetInt32();
                            successRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}");

                            await System.Threading.Tasks.Task.Delay(2000);

                            var task = await _targetContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);

                            task.AssignedTo = assignedTo;
                            task.Details = milestone.Details;
                            task.Progress = progress;
                            //task.PropertiesValues.Add(new PropertiesValue
                            //{
                            //    PropertyId = _configuration.GetSection("TaskProperties:Type").GetValue<int>("PropertyId"),
                            //    Value = mappedType,
                            //    Task = task.Id
                            //});

                            await _targetContext.SaveChangesAsync();
                            break;
                        }
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == -2)
                    {
                        currentRetry++;
                        if (currentRetry >= retryCount)
                        {
                            failedRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, IssueId: {milestone.IssueId}, Error: Execution Timeout after {retryCount} retries.");
                        }
                        else
                        {
                            await System.Threading.Tasks.Task.Delay(1000 * currentRetry);
                        }
                    }
                    catch (Exception ex)
                    {
                        failedRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, IssueId: {milestone.IssueId}, Error: {ex.Message}");
                        break;
                    }
                }
                await System.Threading.Tasks.Task.Delay(2000);
            }

            await SaveRecordsAsync("milestonesSuccessRecords.txt", successRecords);
            await SaveRecordsAsync("milestonesFailedRecords.txt", failedRecords);
            await SaveRecordsAsync("milestonesDateFallbackRecords.txt", dateFallbackRecords);
        }

        private async System.Threading.Tasks.Task<List<DeliverableInfo>> PostDeliverableAsync(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            List<DeliverableInfo> deliverableDetails = new List<DeliverableInfo>();

            var deliverables = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10000 && x.ProjectId == mtProjectId).Select(i => new
            {
                MtDeliverablId = i.IssueId,
                Title = i.Summary,
                Details = i.Description,
                ActualStartDate = i.ActualStartDate,
                PlannedStartDate = i.PlannedStartDate,
                ActualEndDate = i.ActualEndDate,
                PlannedEndDate = i.PlannedEndDate,
                Status = "",
                CompletionPercentage = 0,
                EarnedValue = i.EarnedValue,
                PaymentPlanStatus = i.PaymentPlan,
                AssignedToAccountId = i.CurrentAssigneeAccountId,
                Type = i.Type10074,
                i.IssueStatusName,
                InvoiceNumber = i.InvoiceNumber,
                Amount = i.PlannedValue,
            }).ToList();

            var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

            var deliverablePropsSection = _configuration.GetSection("DeliverableProperties:Type");

            var defaultUserId = _configuration.GetValue<string>("DefaultUserId");

            int propertyId = deliverablePropsSection.GetValue<int>("PropertyId");
            string key = deliverablePropsSection.GetValue<string>("Key");
            string viewType = deliverablePropsSection.GetValue<string>("ViewType");

            // Load existing records from files
            var successRecords = new List<string>(await LoadRecordsAsync("deliverablesSuccessRecords.txt"));
            var failedRecords = new List<string>(await LoadRecordsAsync("deliverablesFailedRecords.txt"));
            var dateFallbackRecords = new List<string>(await LoadRecordsAsync("deliverablesDateFallbackRecords.txt"));

            foreach (var deliverable in deliverables)
            {
                var plannedStartDate = deliverable.PlannedStartDate ?? projectStartDate;
                var plannedFinishDate = deliverable.PlannedEndDate ?? projectFinishDate;
                var actualFinishDate = deliverable.ActualEndDate ?? deliverable.PlannedEndDate ?? projectFinishDate;

                if (deliverable.PlannedStartDate == null || deliverable.PlannedEndDate == null || deliverable.ActualEndDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}, Deliverable ID: {deliverable.MtDeliverablId},  PlannedStartDate: {plannedStartDate}, PlannedEndDate: {plannedFinishDate}, ActualEndDate: {actualFinishDate}");
                }

                var mappedType = deliverable.Type != null && TypeMapping.ContainsKey(deliverable.Type.TrimEnd()) ? TypeMapping[deliverable.Type.TrimEnd()] : null;

                var progress = StatusProgressMapping.ContainsKey(deliverable.IssueStatusName.TrimEnd()) ? StatusProgressMapping[deliverable.IssueStatusName.TrimEnd()] : 0;

                var assignedToEmail = deliverable.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(deliverable.AssignedToAccountId)
                    ? accountIdToEmailMap[deliverable.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Milestone: {deliverable.Title}, Deliverable ID: {deliverable.MtDeliverablId}, AssignedTo: Added default user.");
                }

                var requestBody = new
                {
                    title = deliverable.Title,
                    plannedStartDate = plannedStartDate.ToString("yyyy-MM-dd"),
                    plannedFinishDate = plannedFinishDate.ToString("yyyy-MM-dd"),
                    actualFinishDate = actualFinishDate.ToString("yyyy-MM-dd"),
                    status = deliverable.Status,
                    completionPercentage = progress,
                    attachments = new List<object>(),
                    props = new List<object>
            {
                new
                {
                    id = 0,
                    propertyId,
                    key,
                    value = mappedType,
                    viewType,
                }
            }
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Deliverables", requestBody);

                int retryCount = 3;
                int currentRetry = 0;

                while (currentRetry < retryCount)
                {
                    try
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            using var jsonDoc = JsonDocument.Parse(responseContent);
                            var root = jsonDoc.RootElement;

                            if (root.TryGetProperty("data", out JsonElement dataElement))
                            {
                                successRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}");
                                deliverableDetails.Add(new DeliverableInfo
                                {
                                    MtDeliverablId = deliverable.MtDeliverablId.Value,
                                    Id = dataElement.GetProperty("id").GetInt32(),
                                    Title = dataElement.GetProperty("title").GetString(),
                                    PlannedFinishDate = DateTime.Parse(plannedFinishDate.ToString("yyyy-MM-dd")),
                                    EarnedValue = deliverable.EarnedValue,
                                    PaymentPlanStatus = deliverable.PaymentPlanStatus,
                                    Amount = deliverable.Amount,
                                    InvoiceNumber = deliverable.InvoiceNumber,
                                });

                                var taskId = dataElement.GetProperty("taskId").GetInt32();

                                await System.Threading.Tasks.Task.Delay(2000);

                                var task = await _targetContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                                task.AssignedTo = assignedTo;
                                task.Details = deliverable.Details;
                                task.Progress = progress;
                                //task.PropertiesValues.Add(new PropertiesValue
                                //{
                                //    PropertyId = _configuration.GetSection("TaskProperties:Type").GetValue<int>("PropertyId"),
                                //    Value = mappedType,
                                //    Task = task.Id
                                //});
                                await _targetContext.SaveChangesAsync();
                            }
                            break;
                        }
                        else
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            failedRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}, IssueId: {deliverable.MtDeliverablId}, Error: {errorResponse}");
                            break;
                        }
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == -2)
                    {
                        currentRetry++;
                        if (currentRetry >= retryCount)
                        {
                            failedRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}, IssueId: {deliverable.MtDeliverablId}, Error: Execution Timeout after {retryCount} retries.");
                        }
                        else
                        {
                            await System.Threading.Tasks.Task.Delay(1000 * currentRetry); // Exponential backoff
                        }
                    }
                    catch (Exception ex)
                    {
                        failedRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}, IssueId: {deliverable.MtDeliverablId}, Error: {ex.Message}");
                        break; // No need to retry on other exceptions
                    }
                }
                await System.Threading.Tasks.Task.Delay(2000);
            }

            await SaveRecordsAsync("deliverablesSuccessRecords.txt", successRecords);
            await SaveRecordsAsync("deliverablesFailedRecords.txt", failedRecords);
            await SaveRecordsAsync("deliverablesDateFallbackRecords.txt", dateFallbackRecords);

            return deliverableDetails;
        }

        private async System.Threading.Tasks.Task PostRiskAsync(long mtProjectId, int projectId, List<DeliverableInfo> deliverableInfos, DateTime projectFinishDate, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            var risks = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10078 && x.ProjectId == mtProjectId).Select(i => new
            {
                i.IssueId,
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "Project", 
                Impact = i.Priority,
                Probability = i.Priority, 
                MitigationPlan = "",
                AssignedToAccountId = i.CurrentAssigneeAccountId,
                DueDate = i.DueDate,
                i.ParentIssueId,
            }).ToList();



            var riskPropsSection = _configuration.GetSection("RiskProperties:RelatedDeliverables");

            int propertyId = riskPropsSection.GetValue<int>("PropertyId");
            string key = riskPropsSection.GetValue<string>("Key");
            string valueType = riskPropsSection.GetValue<string>("ValueType");
            string label = riskPropsSection.GetValue<string>("Label");
            var successRecords = new List<string>();
            var failedRecords = new List<string>();
            var dateFallbackRecords = new List<string>();
            var defaultUserId = _configuration.GetValue<string>("DefaultUserId");

            var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

            foreach (var risk in risks)
            {
                var relatedDeliverable = deliverableInfos.FirstOrDefault(x => x.MtDeliverablId == risk.ParentIssueId);

                var dueDate = risk.DueDate ?? projectFinishDate;

                if (risk.DueDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Risk: {risk.Title}, Issue ID: {risk.IssueId},  DueDate: {dueDate}");
                }

                var assignedToEmail = risk.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(risk.AssignedToAccountId)
                    ? accountIdToEmailMap[risk.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Risk: {risk.Title}, Issue ID: {risk.IssueId},   AssignedTo: Added default user.");
                }


                var props = relatedDeliverable != null ? new List<object>
                {
                    new
                    {
                        id = 0,
                        propertyId,
                        key,
                        value = relatedDeliverable.Id.ToString(),
                        viewType = valueType,
                        label
                    }
                } : new List<object>();


                var requestBody = new
                {
                    title = risk.Title,
                    details = risk.Details,
                    category = risk.Category,
                    impact = risk.Impact,
                    probability = risk.Probability,
                    mitigationPlan = risk.MitigationPlan,
                    assignedTo = _configuration.GetValue<string>("DefaultUserId"),
                    dueDate = risk.DueDate.Value.ToString("yyyy-MM-dd"),
                    attachments = new List<object>(),
                    props
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Risks", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    failedRecords.Add($"Project ID: {projectId}, Risk: {risk.Title}, IssueId: {risk.IssueId}, Error: {errorResponse}");
                }
                else
                {
                    successRecords.Add($"Project ID: {projectId}, Risk: {risk.Title}");
                }

                await System.Threading.Tasks.Task.Delay(2000);
            }


            await System.IO.File.WriteAllLinesAsync("risksSuccessRecords.txt", successRecords);
            await System.IO.File.WriteAllLinesAsync("risksFailedRecords.txt", failedRecords);
            await System.IO.File.WriteAllLinesAsync("risksDateFallbackRecords.txt", dateFallbackRecords);
        }

        private async System.Threading.Tasks.Task PostIssueAsync(long mtProjectId, int projectId, List<DeliverableInfo> deliverableInfos, DateTime projectFinishDate, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            var issues = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10009 && x.ProjectId == mtProjectId).Select(i => new
            {
                i.IssueId,
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "",
                Impact = i.Priority,
                ResolutionPlan = "",
                AssignedToAccountId = i.CurrentAssigneeAccountId,
                DueDate = i.DueDate,
                i.ParentIssueId
            }).ToList();

            var issuePropsSection = _configuration.GetSection("IssueProperties:RelatedDeliverables");

            int propertyId = issuePropsSection.GetValue<int>("PropertyId");
            string key = issuePropsSection.GetValue<string>("Key");
            string valueType = issuePropsSection.GetValue<string>("ValueType");
            string label = issuePropsSection.GetValue<string>("Label");

            // Load existing records from files
            var successRecords = new List<string>(await LoadRecordsAsync("issuesSuccessRecords.txt"));
            var failedRecords = new List<string>(await LoadRecordsAsync("issuesFailedRecords.txt"));
            var dateFallbackRecords = new List<string>(await LoadRecordsAsync("issuesDateFallbackRecords.txt"));

            var defaultUserId = _configuration.GetValue<string>("DefaultUserId");
            var userEmailToIdMap = users.ToDictionary(u => u.Email, u => u.UserId);

            foreach (var issue in issues)
            {
                var dueDate = issue.DueDate ?? projectFinishDate;

                if (issue.DueDate == null)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Risk: {issue.Title}, Issue ID: {issue.IssueId}, DueDate: {dueDate}");
                }

                var assignedToEmail = issue.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(issue.AssignedToAccountId)
                    ? accountIdToEmailMap[issue.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Risk: {issue.Title}, Issue ID: {issue.IssueId}, AssignedTo: Added default user.");
                }

                var relatedDeliverable = deliverableInfos.FirstOrDefault(x => x.MtDeliverablId == issue.ParentIssueId);

                var props = relatedDeliverable != null ? new List<object>
        {
            new
            {
                id = 0,
                propertyId,
                key,
                value = relatedDeliverable.Id.ToString(),
                viewType = valueType,
                label
            }
        } : new List<object>();

                var requestBody = new
                {
                    title = issue.Title,
                    details = issue.Details,
                    category = issue.Category,
                    impact = issue.Impact,
                    resolutionPlan = issue.ResolutionPlan,
                    assignedTo,
                    dueDate = dueDate.ToString("yyyy-MM-dd"),
                    attachments = new List<object>(),
                    props,
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Issues", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    failedRecords.Add($"Project ID: {projectId}, Risk: {issue.Title}, Issue ID: {issue.IssueId}, Error: {errorResponse}");
                }
                else
                {
                    successRecords.Add($"Project ID: {projectId}, Risk: {issue.Title}");
                }

                await System.Threading.Tasks.Task.Delay(2000);
            }

            await SaveRecordsAsync("issuesSuccessRecords.txt", successRecords);
            await SaveRecordsAsync("issuesFailedRecords.txt", failedRecords);
            await SaveRecordsAsync("issuesDateFallbackRecords.txt", dateFallbackRecords);
        }


        private async Task<int> FetchPaymentPlanIdAsync(int projectId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/Levels/{projectId}/PaymentPlan");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("data", out JsonElement dataElement) &&
                    dataElement.TryGetProperty("paymentPlanId", out JsonElement paymentPlanIdElement))
                {
                    return paymentPlanIdElement.GetInt32();
                }
                return 0;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return 0;
            }
        }

        public async System.Threading.Tasks.Task ProcessDeliverablesAsync(int projectId, List<DeliverableInfo> deliverables, string token)
        {
            var paymentPlanId = await FetchPaymentPlanIdAsync(projectId, token);

            if (paymentPlanId == 0)
            {
                return;
            }

            var invoiceNumberCounts = new Dictionary<string, int>();
            foreach (var deliverable in deliverables)
            {
                if (!string.IsNullOrEmpty(deliverable.InvoiceNumber))
                {
                    if (!invoiceNumberCounts.ContainsKey(deliverable.InvoiceNumber))
                    {
                        invoiceNumberCounts[deliverable.InvoiceNumber] = 0;
                    }
                    invoiceNumberCounts[deliverable.InvoiceNumber]++;
                    if (invoiceNumberCounts[deliverable.InvoiceNumber] > 1)
                    {
                        deliverable.InvoiceNumber += $"-Shared {invoiceNumberCounts[deliverable.InvoiceNumber]}";
                    }
                }
            }

            foreach (var deliverable in deliverables.Where(d => d.Amount.HasValue && d.Amount != 0))
            {
                await CreateBoqAsync(projectId, deliverable);
                await CreatePaymentPlanItemAsync(projectId, paymentPlanId, deliverable);
            }

            bool shouldSubmitPaymentPlan = deliverables.Any(d => d.PaymentPlanStatus == "Tentative");
            if (!shouldSubmitPaymentPlan)
            {
                var paymentPlanItems = await FetchPaymentPlanItemsAsync(projectId, paymentPlanId);
                await SubmitPaymentPlanAsync(projectId, paymentPlanId, paymentPlanItems);

                foreach (var deliverable in deliverables.Where(x => !string.IsNullOrEmpty(x.InvoiceNumber)))
                {
                    var paymentPlanItem = paymentPlanItems.FirstOrDefault(item => item.Deliverables.Contains(deliverable.Id));
                    if (paymentPlanItem != null && deliverable.EarnedValue.HasValue && deliverable.EarnedValue != 0)
                    {
                        await CreateInvoiceAsync(projectId, deliverable, paymentPlanItem.Id);
                        await SubmitInvoiceAsync(projectId, deliverable);
                    }
                }
            }
        }


        private async System.Threading.Tasks.Task CreateBoqAsync(int projectId, DeliverableInfo deliverable)
        {
            if (!deliverable.Amount.HasValue)
            {
                return;
            }

            var requestBody = new
            {
                itemName = deliverable.Title,
                quantity = 1,
                itemPrice = deliverable.Amount.Value
            };

            var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Boqs", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create BOQ: {errorResponse}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var boq = JsonDocument.Parse(responseContent).RootElement.GetProperty("data");
            deliverable.BoqId = boq.GetProperty("id").GetInt32();
        }

        private async System.Threading.Tasks.Task CreatePaymentPlanItemAsync(int projectId, int paymentPlanId, DeliverableInfo deliverable)
        {
            var requestBody = new
            {
                PaymentPlanId = paymentPlanId,
                Name = deliverable.Title,
                DueDate = deliverable.PlannedFinishDate,
                Deliverables = new List<int> { deliverable.Id },
                Boqs = new List<object> { new { BoqId = deliverable.BoqId, DesiredQuantity = 1 } }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/PaymentPlanItem", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create Payment Plan Item: {errorResponse}");
            }
        }

        private async Task<List<PaymentPlanItemVM>> FetchPaymentPlanItemsAsync(int projectId, int paymentPlanId)
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/Levels/{projectId}/PaymentPlan/{paymentPlanId}/Items");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch Payment Plan Items: {errorResponse}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var paymentPlanItems = JsonDocument.Parse(responseContent).RootElement.GetProperty("data");

            var items = new List<PaymentPlanItemVM>();
            foreach (var item in paymentPlanItems.EnumerateArray())
            {
                items.Add(new PaymentPlanItemVM
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString(),
                    TotalAmount = item.GetProperty("totalAmount").GetProperty("value").GetDouble(),
                    TotalCost = item.GetProperty("totalCost").GetProperty("value").GetDouble(),
                    DueDate = item.GetProperty("dueDate").GetProperty("actualValue").GetDateTime(),
                    Deliverables = item.GetProperty("deliverables").EnumerateArray().Select(d => d.GetProperty("id").GetInt32()).ToList(),
                    Boqs = item.GetProperty("boqs").EnumerateArray().Select(b => b.GetProperty("boqId").GetInt32()).ToList()
                });
            }

            return items;
        }

        private async System.Threading.Tasks.Task SubmitPaymentPlanAsync(int projectId, int paymentPlanId, List<PaymentPlanItemVM> paymentPlanItems)
        {
            var requestBody = new
            {
                items = paymentPlanItems.Select(item => new
                {
                    Id = item.Id,
                    Name = item.Name,
                    TotalAmount = item.TotalAmount,
                    TotalCost = item.TotalCost,
                    DueDate = item.DueDate,
                    Deliverables = item.Deliverables,
                    Boqs = item.Boqs
                }).ToList()
            };

            var submitResponse = await _httpClient.PutAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/PaymentPlan/{paymentPlanId}", requestBody);

            if (!submitResponse.IsSuccessStatusCode)
            {
                var errorResponse = await submitResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to submit Payment Plan: {errorResponse}");
            }
        }

        private async System.Threading.Tasks.Task CreateInvoiceAsync(int projectId, DeliverableInfo deliverable, int paymentPlanItemId)
        {
            var requestBody = new
            {
                deliverable.InvoiceNumber,
                Amount = deliverable.EarnedValue.Value,
                Attachments = new List<object>(),
                PaymentPlanItemId = paymentPlanItemId
            };

            var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/PaymentPlan/Invoices", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var invoice = JsonDocument.Parse(responseContent).RootElement.GetProperty("data");
            deliverable.InvoiceId = invoice.GetProperty("id").GetInt32();
        }

        private async System.Threading.Tasks.Task SubmitInvoiceAsync(int projectId, DeliverableInfo deliverable)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/PaymentPlan/Invoices/Submit/{deliverable.InvoiceId}", new { });

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
            }
        }
        private async System.Threading.Tasks.Task CreateExpensesAsync(int projectId, List<TempoExpensesMt> expenses)
        {
            foreach (var expense in expenses)
            {
                var mappedCategory = MapExpenseCategory(expense.CategoryName);
                var newExpense = new Expense
                {
                    Name = expense.Description,
                    Amount = expense.AmountValue ?? 0,
                    Type = "Indirect",
                    Date = expense.Date,
                    Category = mappedCategory,
                    LevelId = projectId,
                    CreatedAt = DateTime.UtcNow
                };

                _targetContext.Expenses.Add(newExpense);
            }

            await _targetContext.SaveChangesAsync();
        }

        private string MapExpenseCategory(string sourceCategory)
        {
            return sourceCategory switch
            {
                "freelance" => "Freelance",
                "Full time" => "FullTime",
                "license" => "License",
                "Other" => "Other",
                "outsource" => "Outsource",
                "Over time" => "Overtime",
                "Part time" => "PartTime",
                null => "Other",
                _ => "Other"
            };
        }

        private static readonly Dictionary<string, string> TypeMapping = new Dictionary<string, string>
        {
            { "License", "License" },
            { "Scope", "Scope" },
            { "Consultant", "Consultation" },
            { "Outsource", "Outsource" },
            { "Data", "Data" }
        };

        private static readonly Dictionary<string, string> BusinessLineMapping = new Dictionary<string, string>
        {
            { "Closed 2022", "Closed_2022" },
            { "Closed 2023", "Closed_2023" },
            { "Closed 2024", "Closed_2024" },
            { "Consultant", "Consultant" },
            { "internal", "Internal" },
            { "license", "License" },
            { "Outsource", "Outsource" },
            { "pool", "pool" },
            { "Scope", "Scope" },
            { "Support", "Support" }
        };

        private static readonly Dictionary<string, string> CompanyMapping = new Dictionary<string, string>
        {
            { "Adree", "Adree" },
            { "Basser", "Basser" },
            { "Jadaya", "Jadaya" },
            { "Master Team", "Master_Team" },
            { "MW", "MW" }
        };

        private static readonly Dictionary<string, int> StatusProgressMapping = new Dictionary<string, int>
        {
            { "To Do", 5 },
            { "Open", 5 },
            { "Pre-kick off", 5 },
            { "Project Preparation", 5 },
            { "Team allocation", 5 },
            { "Kick-off", 10 },
            { "Configuration", 10 },
            { "Installation and Configuration", 10 },
            { "Business analysis", 20 },
            { "Under Analysis", 20 },
            { "Under Design", 30 },
            { "Design", 30 },
            { "Under Development", 50 },
            { "Development", 50 },
            { "In Progress", 50 },
            { "License Submission", 70 },
            { "UAT", 80 },
            { "Under Review", 80 },
            { "PMO Approval", 90 },
            { "Client approval", 90 },
            { "Awaiting response", 90 },
            { "Deployment", 100 },
            { "Done", 100 },
            { "Deployed", 100 },
            { "On Hold", 0 },
            { "Kick off", 10 }
        };
    }
}
