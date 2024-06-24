using Microsoft.Extensions.Configuration;
using MWDataMigrationApp.Data;
using MWDataMigrationApp.Data.SourceModels;
using MWDataMigrationApp.Data.TargetModels;
using MWDataMigrationApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace MWDataMigrationApp
{
    public class DataMigrationService
    {
        private readonly SourceContext _sourceContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DataMigrationService(SourceContext sourceContext, IConfiguration configuration, HttpClient httpClient)
        {
            _sourceContext = sourceContext;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async System.Threading.Tasks.Task MigrateData()
        {

            string token = await AuthenticateAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var mtProjects = _sourceContext.ProjectsMts.AsEnumerable();

            foreach (var mtProject in mtProjects)
            {
                int projectId = await CreateProjectAsync(mtProject, token);
                int boqId = await CreateDefaultBoqAsync(projectId, token);
                long mtProjectId = mtProject.ProjectId.Value;

                await PostTaskAsync(mtProjectId, projectId, token);
                await PostMilestoneAsync(mtProjectId, projectId, token);
                var  deliverables = await PostDeliverableAsync(mtProjectId, projectId, token);
                await PostRiskAsync(mtProjectId, projectId, token);
                await PostIssueAsync(mtProjectId, projectId, token);

                int paymentPlanId = await FetchPaymentPlanIdAsync(projectId, token);
                await CreatePaymentPlanItemsAsync(projectId, paymentPlanId, token, deliverables, boqId);
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
                var responseData = await response.Content.ReadFromJsonAsync<dynamic>();
                return responseData.data.token;
            }

            throw new Exception("Authentication failed.");
        }

        private async Task<int> CreateProjectAsync(ProjectsMt project, string token)
        {
            var postUri = _configuration["ApiEndpoints:PostProject"];
            var propsConfig = _configuration.GetSection("ProjectProperties");
            var requestBody = new
            {
                name = new { ar = project.Name, en = project.Name },
                props = new List<object>
                {
                    CreateProp(propsConfig.GetSection("StartDate"), DateTime.Now.ToString("yyyy-MM-dd")),
                    CreateProp(propsConfig.GetSection("FinishDate"), DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd")),
                    CreateProp(propsConfig.GetSection("Budget"), ""),
                    CreateProp(propsConfig.GetSection("Manager"), ""),
                    CreateProp(propsConfig.GetSection("ProgramDirector"), ""),
                    CreateProp(propsConfig.GetSection("ClientName"), "Test Client Name"),
                    CreateProp(propsConfig.GetSection("AccountManager"), ""),
                    CreateProp(propsConfig.GetSection("ProjectServices"), "Support"),
                    CreateProp(propsConfig.GetSection("Description"), new { ar = $"<p>{project.Description}</p>", en = $"<p>{project.Description}</p>" })
                },
                levelId = _configuration.GetValue<int>("LevelId"),
                sources = _configuration.GetSection("ProjectSources").Get<List<int>>(),
                attachments = new List<object>(),
                isDraft = false
            };

            var response = await _httpClient.PostAsJsonAsync(postUri, requestBody);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<dynamic>();
                int projectId = responseContent.data;
                return projectId;  
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

        private async System.Threading.Tasks.Task PostTaskAsync(long mtProjectId, int projectId, string token)
        {
            var taskTypeIds = new List<int> { 10007, 10008, 10064, 10111, 10113, 10114, 10117 };
            var tasks = _sourceContext.IssuesMts.Where(x => taskTypeIds.Equals(x.IssueTypeId) && x.ProjectId == mtProjectId).Select(i => new
            {
                Name = i.Summary,
                Details = i.Description,
                StartDate = i.ActualStartDate.Value.ToString("yyyy-MM-dd"),
                FinishDate = i.ActualEndDate.Value.ToString("yyyy-MM-dd"),
                //AssignedTo = i.CurrentAssigneeName,
                Type = "Task"
            }).ToList();

            foreach (var task in tasks)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    name = task.Name,
                    details = task.Details,
                    assignedTo = "",
                    startDate = task.StartDate,
                    finishDate = task.FinishDate,
                    attachments = new List<object>(),
                    status = "",
                    props = new List<object>(),
                    type = task.Type
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Tasks", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post task: {errorResponse}");
                }
            }
        }

        private async System.Threading.Tasks.Task PostMilestoneAsync(long mtProjectId, int projectId, string token)
        {
            var milestoneTypeIds = new List<int> { 10006 };
            var milestones = _sourceContext.IssuesMts.Where(x => milestoneTypeIds.Equals(x.IssueTypeId) && x.ProjectId == mtProjectId).Select(i => new
            {
                Name = i.Summary,
                PlannedStartDate = i.ActualStartDate.Value.ToString("yyyy-MM-dd"),
                PlannedFinishDate = i.ActualEndDate.Value.ToString("yyyy-MM-dd"),
                Weight = 50,
                Status = "" 
            }).ToList();

            foreach (var milestone in milestones)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    name = milestone.Name,
                    plannedStartDate = milestone.PlannedStartDate,
                    plannedFinishDate = milestone.PlannedFinishDate,
                    weight = milestone.Weight,
                    status = milestone.Status,
                    attachments = new List<object>(),
                    props = new List<object>()
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Milestones", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post milestone: {errorResponse}");
                }
            }
        }

        private async System.Threading.Tasks.Task<List<DeliverableInfo>> PostDeliverableAsync(long mtProjectId, int projectId, string token)
        {
            List<DeliverableInfo> deliverableDetails = new List<DeliverableInfo>();

            var deliverableTypeIds = new List<int> { 10000 };
            var deliverables = _sourceContext.IssuesMts.Where(x => deliverableTypeIds.Equals(x.IssueTypeId) && x.ProjectId == mtProjectId).Select(i => new
            {
                Title = i.Summary,
                PlannedStartDate = i.ActualStartDate.Value.ToString("yyyy-MM-dd"),
                PlannedFinishDate = i.ActualEndDate.Value.ToString("yyyy-MM-dd"),
                ActualFinishDate = "", 
                Status = "",
                CompletionPercentage = 0 
            }).ToList();

            foreach (var deliverable in deliverables)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    title = deliverable.Title,
                    plannedStartDate = deliverable.PlannedStartDate,
                    plannedFinishDate = deliverable.PlannedFinishDate,
                    actualFinishDate = deliverable.ActualFinishDate,
                    status = deliverable.Status,
                    completionPercentage = deliverable.CompletionPercentage,
                    attachments = new List<object>(),
                    props = new List<object>()
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Deliverables", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<dynamic>();
                    deliverableDetails.Add(new DeliverableInfo
                    {
                        Id = (int)responseData.data.id,
                        Title = responseData.data.title,
                        PlannedFinishDate = DateTime.Parse(responseData.data.plannedFinishDate.actualValue)
                    });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post deliverable: {errorResponse}");
                }
            }

            return deliverableDetails;
        }

        private async System.Threading.Tasks.Task PostRiskAsync(long mtProjectId, int projectId, string token)
        {
            var riskTypeIds = new List<int> { 10000 };
            var risks = _sourceContext.IssuesMts.Where(x => riskTypeIds.Equals(x.IssueTypeId) && x.ProjectId == mtProjectId).Select(i => new
            {
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "Project", 
                Impact = "Low",
                Probability = i.Priority, 
                MitigationPlan = "",
                AssignedTo = i.CurrentAssigneeName,
                DueDate = i.DueDate.Value.ToString("yyyy-MM-dd")
            }).ToList();

            var riskProps = _configuration.GetSection("RiskProperties:RelatedDeliverables");

            foreach (var risk in risks)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    title = risk.Title,
                    details = risk.Details,
                    category = risk.Category,
                    impact = risk.Impact,
                    probability = risk.Probability,
                    mitigationPlan = risk.MitigationPlan,
                    assignedTo = risk.AssignedTo,
                    dueDate = risk.DueDate,
                    attachments = new List<object>(),
                    props = new List<object>()
                    {
                        new {
                            id = 0,
                            propertyId = riskProps.GetValue<int>("PropertyId"),
                            key = riskProps.GetValue<string>("Key"),
                            value = "dId",
                            viewType = riskProps.GetValue<string>("ValueType")
                        }
                    }
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Risks", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post risk: {errorResponse}");
                }
            }
        }

        private async System.Threading.Tasks.Task PostIssueAsync(long mtProjectId, int projectId, string token)
        {
            var issueTypeIds = new List<int> { 10009 };

            var issues = _sourceContext.IssuesMts.Where(x => issueTypeIds.Equals(x.IssueTypeId) && x.ProjectId == mtProjectId).Select(i => new
            {
                Title = new { ar = i.Summary, en = i.Summary },
                Details = i.Description,
                Category = "",
                Impact = "Low",
                ResolutionPlan = "",
                AssignedTo = i.CurrentAssigneeName,
                DueDate = i.DueDate.Value.ToString("yyyy-MM-dd")
            }).ToList();

            var issueProps = _configuration.GetSection("IssueProperties:RelatedDeliverables");

            foreach (var issue in issues)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    title = issue.Title,
                    details = issue.Details,
                    category = issue.Category,
                    impact = issue.Impact,
                    resolutionPlan = issue.ResolutionPlan,
                    assignedTo = issue.AssignedTo,
                    dueDate = issue.DueDate,
                    attachments = new List<object>(),
                    props = new List<object>()
                    {
                        new {
                            id = 0,
                            propertyId = issueProps.GetValue<int>("PropertyId"),
                            key = issueProps.GetValue<string>("Key"),
                            value = "dId",
                            viewType = issueProps.GetValue<string>("ValueType"),
                            label = issueProps.GetValue<string>("Label")
                        }
                    }
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Issues", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to post issue: {errorResponse}");
                }
            }
        }

        private async Task<int> CreateDefaultBoqAsync(int projectId, string token)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                itemName = "Default BOQ",
                quantity = "100000",
                itemPrice = "0.00000001"
            }), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:PostProject"]}/{projectId}/Boqs", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<dynamic>();
                int boqId = responseContent.data.id;
                return boqId; 
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to create default BOQ: {errorResponse}");
                throw new Exception($"Failed to create default BOQ: {errorResponse}");
            }
        }


        private async Task<int> FetchPaymentPlanIdAsync(int projectId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/Levels/{projectId}/PaymentPlan");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<dynamic>();
                int paymentPlanId = responseContent.data.paymentPlanId;
                return paymentPlanId;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch payment plan: {errorResponse}");
            }
        }

        public async System.Threading.Tasks.Task CreatePaymentPlanItemsAsync(int projectId, int paymentPlanId, string token, IEnumerable<DeliverableInfo> deliverables, int boqId)
        {
            foreach (var deliverable in deliverables)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    PaymentPlanId = paymentPlanId,
                    Name = deliverable.Title,
                    DueDate = deliverable.PlannedFinishDate.ToString("yyyy-MM-dd"),
                    Deliverables = new List<int> { deliverable.Id },
                    Boqs = new List<object>
                    {
                        new { BoqId = boqId, DesiredQuantity = "1" } 
                    }
                }), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/Levels/{projectId}/PaymentPlan/{paymentPlanId}/Items", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create payment plan item for deliverable {deliverable.Id}: {errorResponse}");
                }
            }
        }


    }
}
