using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MWDataMigrationApp.Data;
using MWDataMigrationApp.Data.SourceModels;
using MWDataMigrationApp.Data.TargetModels;
using MWDataMigrationApp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
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

            var domainMts = _sourceContext.DomainMts.ToList();
            //var domainMapping = new Dictionary<string, int>();

            //foreach (var domainMt in domainMts)
            //{
            //    int domainId = await CreateDomainAsync(domainMt, token);
            //    domainMapping[domainMt.DomainName] = domainId;
            //}

            var domainMapping = await GetDomainMappingsAsync();

            var mtProjects = _sourceContext.ProjectsMts.ToList();

            var excludedProjectIds = new HashSet<long> { 10022, 10030, 10040, 10041, 10048, 10061, 10069, 10081, 10082, 10083, 10084, 10085, 10093, 10094, 10095, 10103, 10105, 10073, 10076, 10108, 10110, 10117, 10118, 10121, 10125, 10175, 10181, 10185, 10351 };

            //foreach (var mtProject in mtProjects.Where(x => !excludedProjectIds.Contains(x.ProjectId.Value) && x.ProjectId.HasValue))
                foreach (var mtProject in mtProjects.Where(x => x.ProjectId == 10149))
                {
                var projectDomains = GetDomainsByProject(mtProject.ProjectId.Value);
                var domainIds = projectDomains.Select(d => domainMapping[d.DomainName]).Distinct().ToList();

                int projectId = 334; /*await CreateProjectAsync(mtProject, token, domainIds);*/

                await System.Threading.Tasks.Task.Delay(20000);

                long mtProjectId = mtProject.ProjectId.Value;

                //await PostTaskAsync(mtProjectId, projectId, token, mtProject.StartDate.Value, mtProject.EndDate.Value);
                //await PostMilestoneAsync(mtProjectId, projectId, token, mtProject.StartDate.Value, mtProject.EndDate.Value);
                var  deliverables = await PostDeliverableAsync(mtProjectId, projectId, token, mtProject.StartDate.Value, mtProject.EndDate.Value);

                await ProcessDeliverablesAsync(projectId, deliverables, token);

                await PostRiskAsync(mtProjectId, projectId, token, deliverables, mtProject.EndDate.Value);
                await PostIssueAsync(mtProjectId, projectId, token, deliverables, mtProject.EndDate.Value);

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

        private async Task<int> CreateDomainAsync(DomainMt domainMt, string token)
        {
            var domainUri = _configuration["ApiEndpoints:PostProgram"];
            var propsConfig = _configuration.GetSection("ProgramProperties");
            var requestBody = new
            {
                name = new { ar = domainMt.DomainName, en = domainMt.DomainName },
                props = new List<object>
                {
                    CreateProp(propsConfig.GetSection("Manager"), _configuration.GetValue<string>("DefaultUserId")),
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
                    return dataElement.GetInt32();
                }
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create domain: {errorResponse}");
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

        private async Task<int> CreateProjectAsync(ProjectsMt project, string token, List<int> domainIds)
        {

            var postUri = _configuration["ApiEndpoints:PostProject"];
            var propsConfig = _configuration.GetSection("ProjectProperties");

            var mappedBusinessLine = BusinessLineMapping.ContainsKey(project.BusninessLine.TrimEnd()) ? BusinessLineMapping[project.BusninessLine.TrimEnd()] : null;
            var mappedCompany = CompanyMapping.ContainsKey(project.Company.TrimEnd()) ? CompanyMapping[project.Company.TrimEnd()] :null;

            project.Description = string.IsNullOrEmpty(project.Description) ? "Default Description" : project.Description;
            var requestBody = new
            {
                name = new { ar = project.Name, en = project.Name },
                props = new List<object>
                {
                    CreateProp(propsConfig.GetSection("StartDate"), project.StartDate.Value.ToString("yyyy-MM-dd")),
                    CreateProp(propsConfig.GetSection("FinishDate"), project.EndDate.Value.ToString("yyyy-MM-dd")),
                    CreateProp(propsConfig.GetSection("Budget"), project.ProjectContractValue.Value),
                    CreateProp(propsConfig.GetSection("Manager"), _configuration.GetValue<string>("DefaultUserId")),
                    CreateProp(propsConfig.GetSection("ProgramDirector"), _configuration.GetValue < string >("DefaultUserId")),
                    CreateProp(propsConfig.GetSection("ClientName"), project.Organization),
                    CreateProp(propsConfig.GetSection("AccountManager"), _configuration.GetValue<string>("DefaultUserId")),
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
                    return dataElement.GetInt32();
                }
                throw new Exception("Data property not found in the response.");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create project {project.Name}: {errorResponse}");
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

        private async System.Threading.Tasks.Task PostTaskAsync(long mtProjectId, int projectId, string token, DateTime projectStartDate, DateTime projectFinishDate)
        {
            var taskTypeIds = new List<long> { 10007, 10008, 10064, 10111, 10113, 10114, 10117 };
            var tasks = _sourceContext.IssuesMts.Where(x => taskTypeIds.Contains(x.IssueTypeId.Value) && x.ProjectId == mtProjectId).Select(i => new
            {
                Name = i.Summary,
                Details = i.Description,
                StartDate = i.ActualStartDate.HasValue ? i.ActualStartDate.Value.ToString("yyyy-MM-dd") : projectStartDate.ToString("yyyy-MM-dd"),
                FinishDate = i.ActualEndDate.HasValue ? i.ActualEndDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"),
                AssignedTo = i.CurrentAssigneeName,
                Type = i.Type10074
            }).ToList();

            var taskPropsSection = _configuration.GetSection("TaskProperties:Type");

            int propertyId = taskPropsSection.GetValue<int>("PropertyId");
            string key = taskPropsSection.GetValue<string>("Key");
            string viewType = taskPropsSection.GetValue<string>("ViewType");

            foreach (var task in tasks)
            {
                var mappedType = task.Type != null && TypeMapping.ContainsKey(task.Type.TrimEnd()) ? TypeMapping[task.Type.TrimEnd()] : null;

                var requestBody = new
                {
                    name = task.Name,
                    details = task.Details,
                    assignedTo = _configuration.GetValue<string>("DefaultUserId"),
                    startDate = task.StartDate,
                    finishDate = task.FinishDate,
                    attachments = new List<object>(),
                    status = "",
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post task: {errorResponse}");
                }
                await System.Threading.Tasks.Task.Delay(3000);
            }
        }

        private async System.Threading.Tasks.Task PostMilestoneAsync(long mtProjectId, int projectId, string token, DateTime projectStartDate, DateTime projectFinishDate)
        {
            var milestones = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10006 && x.ProjectId == mtProjectId).Select(i => new
            {
                Name = i.Summary,
                PlannedStartDate = i.ActualStartDate.HasValue ? i.ActualStartDate.Value.ToString("yyyy-MM-dd") : projectStartDate.ToString("yyyy-MM-dd"),
                PlannedFinishDate = i.ActualEndDate.HasValue ? i.ActualEndDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"),
                Weight = 0,
                Status = "",
                Type = i.Type10074
            }).ToList();

            var milestonePropsSection = _configuration.GetSection("MilestoneProperties:Type");

            int propertyId = milestonePropsSection.GetValue<int>("PropertyId");
            string key = milestonePropsSection.GetValue<string>("Key");
            string viewType = milestonePropsSection.GetValue<string>("ViewType");

            foreach (var milestone in milestones)
            {
                var mappedType = milestone.Type != null && TypeMapping.ContainsKey(milestone.Type.TrimEnd()) ? TypeMapping[milestone.Type.TrimEnd()] : null;

                var requestBody = new
                {
                    name = milestone.Name,
                    plannedStartDate = milestone.PlannedStartDate,
                    plannedFinishDate = milestone.PlannedFinishDate,
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post milestone: {errorResponse}");
                }
                await System.Threading.Tasks.Task.Delay(5000);
            }
        }

        private async System.Threading.Tasks.Task<List<DeliverableInfo>> PostDeliverableAsync(long mtProjectId, int projectId, string token, DateTime projectStartDate, DateTime projectFinishDate)
        {
            List<DeliverableInfo> deliverableDetails = new List<DeliverableInfo>();

            var deliverables = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10000 && x.ProjectId == mtProjectId).Select(i => new
            {
                MtDeliverablId = i.IssueId,
                Title = i.Summary,
                PlannedStartDate = i.ActualStartDate.HasValue ? i.ActualStartDate.Value.ToString("yyyy-MM-dd") : projectStartDate.ToString("yyyy-MM-dd"),
                PlannedFinishDate = i.ActualEndDate.HasValue ? i.ActualEndDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"),
                ActualFinishDate = i.ActualEndDate.HasValue ? i.ActualEndDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"), 
                Status = "",
                CompletionPercentage = 0,
                EarnedValue = i.EarnedValue,
                PaymentPlanStatus = i.PaymentPlan,
                Type = i.Type10074,
                InvoiceNumber = i.InvoiceNumber,
                Amount = i.PlannedValue,
            }).ToList();


            var deliverablePropsSection = _configuration.GetSection("DeliverableProperties:Type");

            int propertyId = deliverablePropsSection.GetValue<int>("PropertyId");
            string key = deliverablePropsSection.GetValue<string>("Key");
            string viewType = deliverablePropsSection.GetValue<string>("ViewType");


            foreach (var deliverable in deliverables)
            {
                var mappedType = deliverable.Type != null && TypeMapping.ContainsKey(deliverable.Type.TrimEnd()) ? TypeMapping[deliverable.Type.TrimEnd()] : null;

                var requestBody = new
                {
                    title = deliverable.Title,
                    plannedStartDate = deliverable.PlannedStartDate,
                    plannedFinishDate = deliverable.PlannedFinishDate,
                    actualFinishDate = deliverable.ActualFinishDate,
                    status = deliverable.Status,
                    completionPercentage = deliverable.CompletionPercentage,
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

                if (response.IsSuccessStatusCode)
                {

                    var responseContent = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseContent);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        deliverableDetails.Add(new DeliverableInfo
                        {
                            MtDeliverablId = deliverable.MtDeliverablId.Value,
                            Id = dataElement.GetProperty("id").GetInt32(),
                            Title = dataElement.GetProperty("title").GetString(),
                            PlannedFinishDate = DateTime.Parse(deliverable.PlannedFinishDate),
                            EarnedValue = deliverable.EarnedValue,
                            PaymentPlanStatus = deliverable.PaymentPlanStatus,
                            Amount = deliverable.Amount,
                            InvoiceNumber = deliverable.InvoiceNumber,
                        });
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post deliverable: {errorResponse}");
                }
                await System.Threading.Tasks.Task.Delay(3000);
            }

            return deliverableDetails;
        }

        private async System.Threading.Tasks.Task PostRiskAsync(long mtProjectId, int projectId, string token, List<DeliverableInfo> deliverableInfos, DateTime projectFinishDate)
        {
            var risks = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10078 && x.ProjectId == mtProjectId).Select(i => new
            {
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "Project", 
                Impact = i.Priority,
                Probability = i.Priority, 
                MitigationPlan = "",
                AssignedTo = i.CurrentAssigneeName,
                DueDate = i.DueDate.HasValue ? i.DueDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"),
                i.ParentIssueId,
            }).ToList();


            var riskPropsSection = _configuration.GetSection("RiskProperties:RelatedDeliverables");

            int propertyId = riskPropsSection.GetValue<int>("PropertyId");
            string key = riskPropsSection.GetValue<string>("Key");
            string valueType = riskPropsSection.GetValue<string>("ValueType");
            string label = riskPropsSection.GetValue<string>("Label");

            foreach (var risk in risks)
            {
                var relatedDeliverable = deliverableInfos.FirstOrDefault(x => x.MtDeliverablId == risk.ParentIssueId);

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
                    dueDate = risk.DueDate,
                    attachments = new List<object>(),
                    props
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Risks", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post risk: {errorResponse}");
                }

                await System.Threading.Tasks.Task.Delay(3000);
            }
        }

        private async System.Threading.Tasks.Task PostIssueAsync(long mtProjectId, int projectId, string token, List<DeliverableInfo> deliverableInfos, DateTime projectFinishDate)
        {
            var issues = _sourceContext.IssuesMts.Where(x => x.IssueTypeId == 10009 && x.ProjectId == mtProjectId).Select(i => new
            {
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "",
                Impact = i.Priority,
                ResolutionPlan = "",
                AssignedTo = i.CurrentAssigneeName,
                DueDate = i.DueDate.HasValue ? i.DueDate.Value.ToString("yyyy-MM-dd") : projectFinishDate.ToString("yyyy-MM-dd"),
                i.ParentIssueId
            }).ToList();

            var issuePropsSection = _configuration.GetSection("IssueProperties:RelatedDeliverables");

            int propertyId = issuePropsSection.GetValue<int>("PropertyId");
            string key = issuePropsSection.GetValue<string>("Key");
            string valueType = issuePropsSection.GetValue<string>("ValueType");
            string label = issuePropsSection.GetValue<string>("Label");

            foreach (var issue in issues)
            {
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
                    assignedTo = _configuration.GetValue<string>("DefaultUserId"),
                    dueDate = issue.DueDate,
                    attachments = new List<object>(),
                    props,
                };

                var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Issues", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post issue: {errorResponse}");
                }

                await System.Threading.Tasks.Task.Delay(3000);
            }
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

        private async Task<Dictionary<string, int>> GetDomains(int parentId)
        {
            var levels = await _targetContext.LevelsData.Where(x => x.ParentId == parentId).ToListAsync();
            return levels.ToDictionary(x => x.Name, x => x.Id);
        }

        public async Task<Dictionary<string, int>> GetDomainMappingsAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/charts/Levels/288/cards");
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
        private async Task<HttpResponseMessage> ExecuteApiRequestAsync(string url, HttpMethod method, string token, object data = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (data != null)
            {
                request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            }

            return await _httpClient.SendAsync(request);
        }

        public async System.Threading.Tasks.Task ProcessDeliverablesAsync(int projectId, List<DeliverableInfo> deliverables, string token)
        {
            var paymentPlanId = await FetchPaymentPlanIdAsync(projectId, token);

            if (paymentPlanId == 0)
            {
                return;
            }

            foreach (var deliverable in deliverables.Where(d => d.Amount.HasValue && d.Amount != 0))
            {
                await CreateBoqAsync(projectId, deliverable);
                await CreatePaymentPlanItemAsync(projectId, paymentPlanId, deliverable);
            }

            bool shouldSubmitPaymentPlan = deliverables.Any(d => d.PaymentPlanStatus == "Final" || d.PaymentPlanStatus == "Planned");
            if (shouldSubmitPaymentPlan)
            {
                var paymentPlanItems = await FetchPaymentPlanItemsAsync(projectId, paymentPlanId);
                await SubmitPaymentPlanAsync(projectId, paymentPlanId, paymentPlanItems);

                foreach (var deliverable in deliverables)
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
                InvoiceNumber = deliverable.InvoiceNumber ?? deliverable.BoqId.ToString() ,
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
    }
}
