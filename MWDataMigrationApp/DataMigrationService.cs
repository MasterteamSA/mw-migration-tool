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
using System.Threading.Tasks;
using System.ComponentModel;

namespace MWDataMigrationApp
{
    public class DataMigrationService
    {
        private readonly SourceDbContext _sourceContext;
        private readonly TargetContext _targetContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private  Dictionary<long,(int proId,DateTime? st,DateTime? end)> ProjectMappingDic = new ();
        private readonly List<long> FaildPro =new();
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
            var domainMapping = new Dictionary<string, int>();

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domains_log.txt");
            using (var logWriter = new StreamWriter(logFilePath, append: true))
            {
                foreach (var domainMt in domainMts)
                {
                    int domainId = await CreateDomainAsync(domainMt, accountIdToEmailMap, allRelevantUsers, logWriter);
                    if (domainId != 0)
                    {
                        domainMapping[domainMt.DomainName] = domainId;
                    }
                }
            }

            var mtProjects = _sourceContext.ProjectsMts.ToList();
       
            foreach (var mtProject in mtProjects)
            {
                var projectDomains = GetDomainsByProject(mtProject.ProjectId.Value);
                var domainIds = projectDomains.Select(d => domainMapping[d.DomainName]).Distinct().ToList();
                try
                {
                    var result = await CreateProjectAsync(mtProject, domainIds, userAccounts, allRelevantUsers, projectRoleMembersMts);
                    await System.Threading.Tasks.Task.Delay(1000);
                    result.MtProjectId = mtProject.ProjectId.Value;
                    LogProjectResult(result);
                    int projectId = result.ProjectId;
                    long mtProjectId = mtProject.ProjectId.Value;
                    ProjectMappingDic.Add(mtProjectId, (projectId, result.St, result.En));
                    
                }
                catch (Exception ex)
                {

                }

                 
            }
            var t_result = new List<Data.TargetModels.Task>();
            var m_result = new List<Data.TargetModels.Milestone>();

            foreach (var pro in ProjectMappingDic)
            {
                try
                {
                    //var postTasksResult = await PostTask(pro.Key, pro.Value.proId, pro.Value.st.Value, pro.Value.end.Value, accountIdToEmailMap, allRelevantUsers);
                    //if (postTasksResult != null)
                    //{
                    //    t_result.AddRange(postTasksResult);
                    //}

                    //var postMilestonesResult = await PostMilestoneAsync(pro.Key, pro.Value.proId, pro.Value.st.Value, pro.Value.end.Value, accountIdToEmailMap, allRelevantUsers);
                    //if (postMilestonesResult != null)
                    //{
                    //    m_result.AddRange(postMilestonesResult);
                    //}

                    //var postDeliverablesResult = await PostDeliverableAsync(pro.Key, pro.Value.proId, pro.Value.st.Value, pro.Value.end.Value, accountIdToEmailMap, allRelevantUsers);
                    //if (postDeliverablesResult != null)
                    //{
                    //    await ProcessDeliverablesAsync(pro.Value.proId, postDeliverablesResult, token);
                    //}


                    //await PostRiskAsync(pro.Key, pro.Value.proId, postDeliverablesResult, pro.Value.end.Value, accountIdToEmailMap, allRelevantUsers);
                    await PostIssueAsync(/*pro.Key*/, 1360 /*pro.Value.proId*/, new List<Deliverable>() /*postDeliverablesResult*/, pro.Value.end.Value, accountIdToEmailMap, allRelevantUsers);

                    //postDeliverablesResult.Clear();

                    //var projectExpenses = _sourceContext.TempoExpensesMts.Where(e => e.ProjectId == pro.Key).ToList();

                    //await CreateExpensesAsync(pro.Value.proId, projectExpenses);

                }
                catch (Exception e)
                {

                  
                }
               
            }

            //_targetContext.Tasks.AddRange(t_result);
            //_targetContext.SaveChanges();

            //_targetContext.Milestones.AddRange(m_result);
            //_targetContext.SaveChanges();
        }

        public  void PopuldateDicWithValues()
        {
            var josn = "[\r\n    {\r\n        \"key\": 10022,\r\n        \"proId\": 841,\r\n        \"st\": \"2020-09-20T00:00:00\",\r\n        \"end\": \"2023-01-19T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10030,\r\n        \"proId\": 842,\r\n        \"st\": \"2020-11-17T00:00:00\",\r\n        \"end\": \"2023-07-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10040,\r\n        \"proId\": 843,\r\n        \"st\": \"2021-04-17T00:00:00\",\r\n        \"end\": \"2023-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10041,\r\n        \"proId\": 844,\r\n        \"st\": \"2021-05-30T00:00:00\",\r\n        \"end\": \"2022-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10048,\r\n        \"proId\": 845,\r\n        \"st\": \"2021-08-02T00:00:00\",\r\n        \"end\": \"2022-08-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10061,\r\n        \"proId\": 846,\r\n        \"st\": \"2021-11-22T00:00:00\",\r\n        \"end\": \"2023-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10069,\r\n        \"proId\": 847,\r\n        \"st\": \"2021-10-17T00:00:00\",\r\n        \"end\": \"2022-04-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10081,\r\n        \"proId\": 848,\r\n        \"st\": \"2021-10-20T00:00:00\",\r\n        \"end\": \"2024-03-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10082,\r\n        \"proId\": 849,\r\n        \"st\": \"2022-10-09T00:00:00\",\r\n        \"end\": \"2024-06-13T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10083,\r\n        \"proId\": 850,\r\n        \"st\": \"2022-05-01T00:00:00\",\r\n        \"end\": \"2025-05-22T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10084,\r\n        \"proId\": 851,\r\n        \"st\": \"2022-07-01T00:00:00\",\r\n        \"end\": \"2023-03-16T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10085,\r\n        \"proId\": 852,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10093,\r\n        \"proId\": 853,\r\n        \"st\": \"2022-07-26T00:00:00\",\r\n        \"end\": \"2023-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10094,\r\n        \"proId\": 854,\r\n        \"st\": \"2022-08-01T00:00:00\",\r\n        \"end\": \"2023-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10095,\r\n        \"proId\": 855,\r\n        \"st\": \"2022-08-03T00:00:00\",\r\n        \"end\": \"2023-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10103,\r\n        \"proId\": 856,\r\n        \"st\": \"2022-10-02T00:00:00\",\r\n        \"end\": \"2023-10-15T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10105,\r\n        \"proId\": 857,\r\n        \"st\": \"2022-12-11T00:00:00\",\r\n        \"end\": \"2026-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10073,\r\n        \"proId\": 858,\r\n        \"st\": \"2022-02-06T00:00:00\",\r\n        \"end\": \"2023-08-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10076,\r\n        \"proId\": 859,\r\n        \"st\": \"2022-03-10T00:00:00\",\r\n        \"end\": \"2022-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10108,\r\n        \"proId\": 860,\r\n        \"st\": \"2022-10-02T00:00:00\",\r\n        \"end\": \"2023-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10110,\r\n        \"proId\": 861,\r\n        \"st\": \"2022-10-30T00:00:00\",\r\n        \"end\": \"2023-01-19T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10117,\r\n        \"proId\": 862,\r\n        \"st\": \"2022-12-12T00:00:00\",\r\n        \"end\": \"2023-10-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10118,\r\n        \"proId\": 863,\r\n        \"st\": \"2022-12-25T00:00:00\",\r\n        \"end\": \"2023-05-15T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10121,\r\n        \"proId\": 864,\r\n        \"st\": \"2022-09-01T00:00:00\",\r\n        \"end\": \"2023-05-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10125,\r\n        \"proId\": 865,\r\n        \"st\": \"2023-02-12T00:00:00\",\r\n        \"end\": \"2023-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10175,\r\n        \"proId\": 866,\r\n        \"st\": \"2023-03-26T00:00:00\",\r\n        \"end\": \"2025-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10181,\r\n        \"proId\": 867,\r\n        \"st\": \"2023-11-25T00:00:00\",\r\n        \"end\": \"2024-08-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10185,\r\n        \"proId\": 868,\r\n        \"st\": \"2022-07-31T00:00:00\",\r\n        \"end\": \"2024-07-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10351,\r\n        \"proId\": 869,\r\n        \"st\": \"2024-04-14T00:00:00\",\r\n        \"end\": \"2024-06-04T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10162,\r\n        \"proId\": 870,\r\n        \"st\": \"2023-11-05T00:00:00\",\r\n        \"end\": \"2026-08-26T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10021,\r\n        \"proId\": 871,\r\n        \"st\": \"2021-05-01T00:00:00\",\r\n        \"end\": \"2024-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10087,\r\n        \"proId\": 872,\r\n        \"st\": \"2022-04-03T00:00:00\",\r\n        \"end\": \"2023-04-04T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10031,\r\n        \"proId\": 873,\r\n        \"st\": \"2021-03-07T00:00:00\",\r\n        \"end\": \"2023-04-02T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10059,\r\n        \"proId\": 874,\r\n        \"st\": \"2021-07-04T00:00:00\",\r\n        \"end\": \"2023-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10044,\r\n        \"proId\": 875,\r\n        \"st\": \"2021-05-31T00:00:00\",\r\n        \"end\": \"2023-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10068,\r\n        \"proId\": 876,\r\n        \"st\": \"2021-12-02T00:00:00\",\r\n        \"end\": \"2023-07-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10066,\r\n        \"proId\": 877,\r\n        \"st\": \"2021-11-28T00:00:00\",\r\n        \"end\": \"2022-09-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10088,\r\n        \"proId\": 878,\r\n        \"st\": \"2022-04-17T00:00:00\",\r\n        \"end\": \"2023-04-20T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10107,\r\n        \"proId\": 879,\r\n        \"st\": \"2022-10-09T00:00:00\",\r\n        \"end\": \"2023-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10240,\r\n        \"proId\": 880,\r\n        \"st\": \"2024-03-24T00:00:00\",\r\n        \"end\": \"2024-03-24T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10104,\r\n        \"proId\": 881,\r\n        \"st\": \"2023-03-26T00:00:00\",\r\n        \"end\": \"2026-03-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10238,\r\n        \"proId\": 882,\r\n        \"st\": \"2022-10-31T00:00:00\",\r\n        \"end\": \"2023-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10239,\r\n        \"proId\": 883,\r\n        \"st\": \"2024-01-25T00:00:00\",\r\n        \"end\": \"2025-07-15T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10182,\r\n        \"proId\": 884,\r\n        \"st\": \"2023-01-31T00:00:00\",\r\n        \"end\": \"2023-04-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10037,\r\n        \"proId\": 885,\r\n        \"st\": \"2021-03-03T00:00:00\",\r\n        \"end\": \"2022-11-15T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10130,\r\n        \"proId\": 886,\r\n        \"st\": \"2023-04-12T00:00:00\",\r\n        \"end\": \"2024-05-14T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10153,\r\n        \"proId\": 887,\r\n        \"st\": \"2023-09-17T00:00:00\",\r\n        \"end\": \"2024-03-07T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10128,\r\n        \"proId\": 888,\r\n        \"st\": \"2023-05-21T00:00:00\",\r\n        \"end\": \"2024-02-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10106,\r\n        \"proId\": 889,\r\n        \"st\": \"2022-09-25T00:00:00\",\r\n        \"end\": \"2024-09-14T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10140,\r\n        \"proId\": 890,\r\n        \"st\": \"2023-07-25T00:00:00\",\r\n        \"end\": \"2025-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10183,\r\n        \"proId\": 891,\r\n        \"st\": \"2023-04-25T00:00:00\",\r\n        \"end\": \"2024-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10151,\r\n        \"proId\": 892,\r\n        \"st\": \"2023-09-03T00:00:00\",\r\n        \"end\": \"2026-08-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10241,\r\n        \"proId\": 893,\r\n        \"st\": \"2024-01-01T00:00:00\",\r\n        \"end\": \"2025-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10242,\r\n        \"proId\": 894,\r\n        \"st\": \"2023-02-21T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10126,\r\n        \"proId\": 895,\r\n        \"st\": \"2023-05-07T00:00:00\",\r\n        \"end\": \"2024-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10152,\r\n        \"proId\": 896,\r\n        \"st\": \"2023-10-01T00:00:00\",\r\n        \"end\": \"2024-04-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10123,\r\n        \"proId\": 897,\r\n        \"st\": \"2023-01-29T00:00:00\",\r\n        \"end\": \"2024-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10247,\r\n        \"proId\": 898,\r\n        \"st\": \"2024-04-25T00:00:00\",\r\n        \"end\": \"2024-06-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10122,\r\n        \"proId\": 899,\r\n        \"st\": \"2023-01-23T00:00:00\",\r\n        \"end\": \"2024-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10080,\r\n        \"proId\": 900,\r\n        \"st\": \"2022-03-17T00:00:00\",\r\n        \"end\": \"2025-03-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10244,\r\n        \"proId\": 901,\r\n        \"st\": \"2023-03-15T00:00:00\",\r\n        \"end\": \"2023-03-20T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10245,\r\n        \"proId\": 902,\r\n        \"st\": \"2024-04-26T00:00:00\",\r\n        \"end\": \"2025-05-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10139,\r\n        \"proId\": 903,\r\n        \"st\": \"2023-07-23T00:00:00\",\r\n        \"end\": \"2024-07-22T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10127,\r\n        \"proId\": 904,\r\n        \"st\": \"2023-05-10T00:00:00\",\r\n        \"end\": \"2023-11-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10136,\r\n        \"proId\": 905,\r\n        \"st\": \"2023-07-24T00:00:00\",\r\n        \"end\": \"2025-07-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10180,\r\n        \"proId\": 906,\r\n        \"st\": \"2022-11-25T00:00:00\",\r\n        \"end\": \"2023-08-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10129,\r\n        \"proId\": 907,\r\n        \"st\": \"2023-06-01T00:00:00\",\r\n        \"end\": \"2023-09-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10246,\r\n        \"proId\": 908,\r\n        \"st\": \"2022-12-05T00:00:00\",\r\n        \"end\": \"2023-12-14T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10184,\r\n        \"proId\": 909,\r\n        \"st\": \"2023-12-25T00:00:00\",\r\n        \"end\": \"2023-12-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10187,\r\n        \"proId\": 910,\r\n        \"st\": \"2023-08-20T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10149,\r\n        \"proId\": 911,\r\n        \"st\": \"2023-09-03T00:00:00\",\r\n        \"end\": \"2024-05-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10251,\r\n        \"proId\": 912,\r\n        \"st\": \"2023-03-30T00:00:00\",\r\n        \"end\": \"2024-03-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10144,\r\n        \"proId\": 913,\r\n        \"st\": \"2023-08-23T00:00:00\",\r\n        \"end\": \"2023-11-19T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10248,\r\n        \"proId\": 914,\r\n        \"st\": \"2023-09-23T00:00:00\",\r\n        \"end\": \"2024-11-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10186,\r\n        \"proId\": 915,\r\n        \"st\": \"2023-04-06T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10135,\r\n        \"proId\": 916,\r\n        \"st\": \"2023-07-09T00:00:00\",\r\n        \"end\": \"2023-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10249,\r\n        \"proId\": 917,\r\n        \"st\": \"2023-01-19T00:00:00\",\r\n        \"end\": \"2024-01-18T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10116,\r\n        \"proId\": 918,\r\n        \"st\": \"2022-12-18T00:00:00\",\r\n        \"end\": \"2023-11-27T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10150,\r\n        \"proId\": 919,\r\n        \"st\": \"2023-08-24T00:00:00\",\r\n        \"end\": \"2024-06-04T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10268,\r\n        \"proId\": 920,\r\n        \"st\": \"2023-02-22T00:00:00\",\r\n        \"end\": \"2024-02-21T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10258,\r\n        \"proId\": 921,\r\n        \"st\": \"2024-01-25T00:00:00\",\r\n        \"end\": \"2024-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10219,\r\n        \"proId\": 922,\r\n        \"st\": \"2024-03-03T00:00:00\",\r\n        \"end\": \"2024-05-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10262,\r\n        \"proId\": 923,\r\n        \"st\": \"2023-08-24T00:00:00\",\r\n        \"end\": \"2023-08-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10253,\r\n        \"proId\": 924,\r\n        \"st\": \"2023-04-25T00:00:00\",\r\n        \"end\": \"2025-02-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10271,\r\n        \"proId\": 925,\r\n        \"st\": \"2024-03-01T00:00:00\",\r\n        \"end\": \"2024-03-06T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10265,\r\n        \"proId\": 926,\r\n        \"st\": \"2023-05-31T00:00:00\",\r\n        \"end\": \"2023-06-05T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10269,\r\n        \"proId\": 927,\r\n        \"st\": \"2023-05-13T00:00:00\",\r\n        \"end\": \"2024-05-12T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10270,\r\n        \"proId\": 928,\r\n        \"st\": \"2023-10-05T00:00:00\",\r\n        \"end\": \"2023-10-10T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10260,\r\n        \"proId\": 929,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10264,\r\n        \"proId\": 930,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10259,\r\n        \"proId\": 931,\r\n        \"st\": \"2023-04-29T00:00:00\",\r\n        \"end\": \"2024-04-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10254,\r\n        \"proId\": 932,\r\n        \"st\": \"2023-04-24T00:00:00\",\r\n        \"end\": \"2025-04-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10146,\r\n        \"proId\": 933,\r\n        \"st\": \"2023-08-21T00:00:00\",\r\n        \"end\": \"2024-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10231,\r\n        \"proId\": 934,\r\n        \"st\": \"2024-01-31T00:00:00\",\r\n        \"end\": \"2025-07-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10255,\r\n        \"proId\": 935,\r\n        \"st\": \"2023-11-25T00:00:00\",\r\n        \"end\": \"2024-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10256,\r\n        \"proId\": 936,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10257,\r\n        \"proId\": 937,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10252,\r\n        \"proId\": 938,\r\n        \"st\": \"2023-03-06T00:00:00\",\r\n        \"end\": \"2024-03-07T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10368,\r\n        \"proId\": 939,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10163,\r\n        \"proId\": 940,\r\n        \"st\": \"2023-10-15T00:00:00\",\r\n        \"end\": \"2024-04-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10267,\r\n        \"proId\": 941,\r\n        \"st\": \"2023-06-30T00:00:00\",\r\n        \"end\": \"2023-07-05T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10169,\r\n        \"proId\": 942,\r\n        \"st\": \"2023-12-21T00:00:00\",\r\n        \"end\": \"2025-12-21T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10201,\r\n        \"proId\": 943,\r\n        \"st\": \"2023-12-24T00:00:00\",\r\n        \"end\": \"2024-10-23T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10261,\r\n        \"proId\": 944,\r\n        \"st\": \"2023-08-15T00:00:00\",\r\n        \"end\": \"2023-08-20T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10312,\r\n        \"proId\": 945,\r\n        \"st\": \"2024-01-23T00:00:00\",\r\n        \"end\": \"2024-01-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10307,\r\n        \"proId\": 946,\r\n        \"st\": \"2023-10-19T00:00:00\",\r\n        \"end\": \"2023-10-24T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10167,\r\n        \"proId\": 947,\r\n        \"st\": \"2023-11-01T00:00:00\",\r\n        \"end\": \"2024-06-03T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10143,\r\n        \"proId\": 948,\r\n        \"st\": \"2023-06-06T00:00:00\",\r\n        \"end\": \"2024-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10166,\r\n        \"proId\": 949,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10284,\r\n        \"proId\": 950,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10280,\r\n        \"proId\": 951,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10306,\r\n        \"proId\": 952,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10145,\r\n        \"proId\": 953,\r\n        \"st\": \"2023-08-06T00:00:00\",\r\n        \"end\": \"2025-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10156,\r\n        \"proId\": 954,\r\n        \"st\": \"2023-10-01T00:00:00\",\r\n        \"end\": \"2025-06-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10228,\r\n        \"proId\": 955,\r\n        \"st\": \"2024-01-16T00:00:00\",\r\n        \"end\": \"2024-12-16T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10278,\r\n        \"proId\": 956,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10282,\r\n        \"proId\": 957,\r\n        \"st\": \"2023-12-20T00:00:00\",\r\n        \"end\": \"2023-12-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10272,\r\n        \"proId\": 958,\r\n        \"st\": \"2024-02-20T00:00:00\",\r\n        \"end\": \"2024-02-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10285,\r\n        \"proId\": 959,\r\n        \"st\": \"2024-01-24T00:00:00\",\r\n        \"end\": \"2024-01-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10286,\r\n        \"proId\": 960,\r\n        \"st\": \"2024-05-28T00:00:00\",\r\n        \"end\": \"2024-05-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10275,\r\n        \"proId\": 961,\r\n        \"st\": \"2023-04-25T00:00:00\",\r\n        \"end\": \"2024-11-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10210,\r\n        \"proId\": 962,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10147,\r\n        \"proId\": 963,\r\n        \"st\": \"2023-08-27T00:00:00\",\r\n        \"end\": \"2024-02-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10170,\r\n        \"proId\": 964,\r\n        \"st\": \"2023-11-30T00:00:00\",\r\n        \"end\": \"2024-08-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10229,\r\n        \"proId\": 965,\r\n        \"st\": \"2024-04-15T00:00:00\",\r\n        \"end\": \"2024-06-05T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10164,\r\n        \"proId\": 966,\r\n        \"st\": \"2023-11-01T00:00:00\",\r\n        \"end\": \"2024-12-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10330,\r\n        \"proId\": 967,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10273,\r\n        \"proId\": 968,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10281,\r\n        \"proId\": 969,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10277,\r\n        \"proId\": 970,\r\n        \"st\": \"2023-07-30T00:00:00\",\r\n        \"end\": \"2024-07-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10218,\r\n        \"proId\": 971,\r\n        \"st\": \"2024-01-17T00:00:00\",\r\n        \"end\": \"2024-07-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10232,\r\n        \"proId\": 972,\r\n        \"st\": \"2024-02-19T00:00:00\",\r\n        \"end\": \"2024-05-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10195,\r\n        \"proId\": 973,\r\n        \"st\": \"2023-11-01T00:00:00\",\r\n        \"end\": \"2025-08-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10274,\r\n        \"proId\": 974,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10205,\r\n        \"proId\": 975,\r\n        \"st\": \"2024-03-03T00:00:00\",\r\n        \"end\": \"2024-06-04T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10283,\r\n        \"proId\": 976,\r\n        \"st\": \"2023-12-20T00:00:00\",\r\n        \"end\": \"2023-12-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10309,\r\n        \"proId\": 977,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10276,\r\n        \"proId\": 978,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10279,\r\n        \"proId\": 979,\r\n        \"st\": \"2024-03-15T00:00:00\",\r\n        \"end\": \"2024-03-20T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10287,\r\n        \"proId\": 980,\r\n        \"st\": \"2023-09-13T00:00:00\",\r\n        \"end\": \"2024-09-13T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10310,\r\n        \"proId\": 981,\r\n        \"st\": \"2023-10-19T00:00:00\",\r\n        \"end\": \"2023-10-24T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10165,\r\n        \"proId\": 982,\r\n        \"st\": \"2023-10-01T00:00:00\",\r\n        \"end\": \"2024-10-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10217,\r\n        \"proId\": 983,\r\n        \"st\": \"2024-02-15T00:00:00\",\r\n        \"end\": \"2024-06-05T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10328,\r\n        \"proId\": 984,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10290,\r\n        \"proId\": 985,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10300,\r\n        \"proId\": 986,\r\n        \"st\": \"2024-03-27T00:00:00\",\r\n        \"end\": \"2025-03-27T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10155,\r\n        \"proId\": 987,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10352,\r\n        \"proId\": 988,\r\n        \"st\": \"2024-04-22T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10323,\r\n        \"proId\": 989,\r\n        \"st\": \"2024-06-22T00:00:00\",\r\n        \"end\": \"2028-05-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10233,\r\n        \"proId\": 990,\r\n        \"st\": \"2024-02-19T00:00:00\",\r\n        \"end\": \"2024-02-19T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10200,\r\n        \"proId\": 991,\r\n        \"st\": \"2024-01-29T00:00:00\",\r\n        \"end\": \"2024-12-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10202,\r\n        \"proId\": 992,\r\n        \"st\": \"2024-08-26T00:00:00\",\r\n        \"end\": \"2026-03-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10293,\r\n        \"proId\": 993,\r\n        \"st\": \"2023-03-30T00:00:00\",\r\n        \"end\": \"2026-03-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10305,\r\n        \"proId\": 994,\r\n        \"st\": \"2023-10-19T00:00:00\",\r\n        \"end\": \"2023-10-24T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10178,\r\n        \"proId\": 995,\r\n        \"st\": \"2023-12-17T00:00:00\",\r\n        \"end\": \"2024-07-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10288,\r\n        \"proId\": 996,\r\n        \"st\": \"2024-03-15T00:00:00\",\r\n        \"end\": \"2024-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10308,\r\n        \"proId\": 997,\r\n        \"st\": \"2023-10-20T00:00:00\",\r\n        \"end\": \"2023-10-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10302,\r\n        \"proId\": 998,\r\n        \"st\": \"2024-05-25T00:00:00\",\r\n        \"end\": \"2026-05-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10176,\r\n        \"proId\": 999,\r\n        \"st\": \"2023-12-12T00:00:00\",\r\n        \"end\": \"2024-06-05T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10301,\r\n        \"proId\": 1000,\r\n        \"st\": \"2024-04-13T00:00:00\",\r\n        \"end\": \"2025-02-28T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10369,\r\n        \"proId\": 1001,\r\n        \"st\": \"2024-05-27T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10211,\r\n        \"proId\": 1002,\r\n        \"st\": \"2024-04-01T00:00:00\",\r\n        \"end\": \"2025-03-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10292,\r\n        \"proId\": 1003,\r\n        \"st\": \"2023-12-30T00:00:00\",\r\n        \"end\": \"2024-01-04T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10298,\r\n        \"proId\": 0,\r\n        \"st\": \"2023-11-30T00:00:00\",\r\n        \"end\": \"2024-11-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10335,\r\n        \"proId\": 0,\r\n        \"st\": \"2024-03-20T00:00:00\",\r\n        \"end\": \"2024-03-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10215,\r\n        \"proId\": 1004,\r\n        \"st\": \"2023-10-29T00:00:00\",\r\n        \"end\": \"2024-10-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10339,\r\n        \"proId\": 1005,\r\n        \"st\": \"2024-05-05T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10325,\r\n        \"proId\": 1006,\r\n        \"st\": \"2024-05-03T00:00:00\",\r\n        \"end\": \"2024-05-08T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10294,\r\n        \"proId\": 1007,\r\n        \"st\": \"2024-01-26T00:00:00\",\r\n        \"end\": \"2024-01-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10311,\r\n        \"proId\": 1008,\r\n        \"st\": \"2023-12-02T00:00:00\",\r\n        \"end\": \"2023-12-07T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10333,\r\n        \"proId\": 1009,\r\n        \"st\": \"2024-03-19T00:00:00\",\r\n        \"end\": \"2024-03-24T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10299,\r\n        \"proId\": 1010,\r\n        \"st\": \"2023-12-09T00:00:00\",\r\n        \"end\": \"2023-12-14T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10345,\r\n        \"proId\": 1011,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10222,\r\n        \"proId\": 1012,\r\n        \"st\": \"2024-05-02T00:00:00\",\r\n        \"end\": \"2025-06-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10373,\r\n        \"proId\": 1013,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10338,\r\n        \"proId\": 1014,\r\n        \"st\": \"2024-05-25T00:00:00\",\r\n        \"end\": \"2024-10-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10344,\r\n        \"proId\": 1015,\r\n        \"st\": \"2024-10-25T00:00:00\",\r\n        \"end\": \"2025-09-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10331,\r\n        \"proId\": 1016,\r\n        \"st\": \"2024-05-31T00:00:00\",\r\n        \"end\": \"2026-06-01T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10220,\r\n        \"proId\": 1017,\r\n        \"st\": \"2024-01-03T00:00:00\",\r\n        \"end\": \"2025-01-03T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10224,\r\n        \"proId\": 1018,\r\n        \"st\": \"2024-03-26T00:00:00\",\r\n        \"end\": \"2024-09-11T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10234,\r\n        \"proId\": 1019,\r\n        \"st\": \"2024-03-06T00:00:00\",\r\n        \"end\": \"2024-03-11T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10225,\r\n        \"proId\": 1020,\r\n        \"st\": \"2024-03-14T00:00:00\",\r\n        \"end\": \"2024-06-16T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10313,\r\n        \"proId\": 1021,\r\n        \"st\": \"2024-01-24T00:00:00\",\r\n        \"end\": \"2024-01-29T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10303,\r\n        \"proId\": 1022,\r\n        \"st\": \"2024-03-16T00:00:00\",\r\n        \"end\": \"2024-03-21T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10291,\r\n        \"proId\": 1023,\r\n        \"st\": \"2024-02-03T00:00:00\",\r\n        \"end\": \"2024-03-17T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10304,\r\n        \"proId\": 1024,\r\n        \"st\": \"2024-02-11T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10372,\r\n        \"proId\": 1025,\r\n        \"st\": \"2024-05-30T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10337,\r\n        \"proId\": 1026,\r\n        \"st\": \"2024-03-24T00:00:00\",\r\n        \"end\": \"2024-09-16T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10358,\r\n        \"proId\": 1027,\r\n        \"st\": \"2024-04-27T00:00:00\",\r\n        \"end\": \"2024-05-02T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10364,\r\n        \"proId\": 1028,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10357,\r\n        \"proId\": 1029,\r\n        \"st\": \"2024-09-25T00:00:00\",\r\n        \"end\": \"2025-06-30T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10346,\r\n        \"proId\": 1030,\r\n        \"st\": \"2024-04-18T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10371,\r\n        \"proId\": 1031,\r\n        \"st\": null,\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10332,\r\n        \"proId\": 1032,\r\n        \"st\": \"2024-04-01T00:00:00\",\r\n        \"end\": \"2024-06-17T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10336,\r\n        \"proId\": 1033,\r\n        \"st\": \"2024-03-20T00:00:00\",\r\n        \"end\": \"2024-03-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10334,\r\n        \"proId\": 1034,\r\n        \"st\": \"2024-03-20T00:00:00\",\r\n        \"end\": \"2024-03-25T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10353,\r\n        \"proId\": 1035,\r\n        \"st\": \"2024-04-23T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10356,\r\n        \"proId\": 1036,\r\n        \"st\": \"2024-04-18T00:00:00\",\r\n        \"end\": \"2024-04-23T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10362,\r\n        \"proId\": 1037,\r\n        \"st\": \"2024-05-07T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10363,\r\n        \"proId\": 1038,\r\n        \"st\": \"2024-05-26T00:00:00\",\r\n        \"end\": \"2024-05-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10366,\r\n        \"proId\": 1039,\r\n        \"st\": \"2024-05-02T00:00:00\",\r\n        \"end\": \"2024-05-07T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10365,\r\n        \"proId\": 1040,\r\n        \"st\": \"2024-05-26T00:00:00\",\r\n        \"end\": \"2024-05-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10367,\r\n        \"proId\": 1041,\r\n        \"st\": \"2024-05-26T00:00:00\",\r\n        \"end\": \"2024-05-31T00:00:00\"\r\n    },\r\n    {\r\n        \"key\": 10370,\r\n        \"proId\": 1042,\r\n        \"st\": \"2024-06-10T00:00:00\",\r\n        \"end\": null\r\n    },\r\n    {\r\n        \"key\": 10374,\r\n        \"proId\": 1043,\r\n        \"st\": \"2024-06-03T00:00:00\",\r\n        \"end\": null\r\n    }\r\n]";
             var options = new JsonSerializerOptions
            {
                Converters = { new DateTimeConverter() },
                PropertyNameCaseInsensitive = true
            };
            var projectMappingDic = System.Text.Json.JsonSerializer.Deserialize<List<ProjectData>>(josn, options);

            // Example usage of the dictionary
            foreach (var item in projectMappingDic)
            {
                if (item.proId < 1)
                    continue;
                ProjectMappingDic.Add(item.key,(item.proId, item.st??DateTime.Now.AddDays(-1), item.end??DateTime.Now));
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
            var response = await _httpClient.GetAsync($"{_configuration["ApiEndpoints:BaseUrl"]}/charts/Levels/424/cards");
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
                        DefaultAccountManagerAdded = defaultAccountManagerAdded,
                        St = DateTime.Parse(startDate),
                        En = DateTime.Parse(endDate),
                    };
                }
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
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
            var logMessage = $"Project ID: {result.ProjectId},MtProject ID: {result.MtProjectId}, Success: {result.ProjectId > 0}, " +
                             $"Default Start Date Added: {result.DefaultStartDateAdded}, Default End Date Added: {result.DefaultEndDateAdded}, " +
                             $"Default Manager Added: {result.DefaultManagerAdded}, Default Program Director Added: {result.DefaultProgramDirectorAdded}, " +
                             $"Default Account Manager Added: {result.DefaultAccountManagerAdded}";
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "project_log.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {logMessage}");
            }
        }
        private void LogProjectJsonResult()
        {
            
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"project_log{DateTime.Now.Ticks}.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(ProjectMappingDic);
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

        private async System.Threading.Tasks.Task<List<Data.TargetModels.Task>> PostTask(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate,
     Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            try
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
                var tempGuid = tasks.Where(x => x.ParentIssueId is not null).GroupBy(x => x.ParentIssueId).ToDictionary(x => x.Key, x => Guid.NewGuid().ToString());
                var parentTaskMapping = new Dictionary<long, string>();

                var defaultUserId = _configuration.GetValue<string>("DefaultUserId");
                var TasksList = new List<Data.TargetModels.Task>();
                // Load existing records from files
                var successRecords = new List<string>(await LoadRecordsAsync("tasksSuccessRecords.txt"));
                var failedRecords = new List<string>(await LoadRecordsAsync("tasksFailedRecords.txt"));
                var dateFallbackRecords = new List<string>(await LoadRecordsAsync("tasksDateFallbackRecords.txt"));

                // create parent tasks
                foreach (var parentTask in tasks)
                {
                    var startDate = parentTask.ActualStartDate ?? parentTask.PlannedStartDate ?? projectStartDate;
                    var finishDate = parentTask.ActualEndDate ?? parentTask.PlannedEndDate ?? projectFinishDate;

                    if (parentTask.ActualStartDate == null || parentTask.ActualEndDate == null)
                    {
                        dateFallbackRecords.Add($"Project ID: {projectId}, Task: {parentTask.Name}, Issue ID: {parentTask.IssueId}, StartDate: {startDate}, FinishDate: {finishDate}");
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
                        failedRecords.Add($"Project ID: {projectId},  Task: {parentTask.Name}, Issue ID: {parentTask.IssueId} , AssignedTo: Added default user.");
                    }

                    TasksList.Add(new Data.TargetModels.Task
                    {
                        LevelId = projectId,
                        LogId = 1,
                        Guid = tempGuid.ContainsKey(parentTask.IssueId) && parentTask.IssueId is not null ? tempGuid[parentTask.IssueId] : Guid.NewGuid().ToString(),
                        ParentGuid = parentTask.ParentIssueId is not null  && tempGuid.ContainsKey(parentTask.ParentIssueId)? tempGuid[parentTask.ParentIssueId] : null,
                        Title = parentTask.Name,
                        Details = parentTask.Details,
                        AssignedTo = assignedTo,
                        StartDate = startDate.DateTime,
                        FinishDate = finishDate.DateTime,
                        Attachments = "[]",
                        Status = "",
                        Progress = progress,
                        TaskType = "Task",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = defaultUserId,
                        PropertiesValues = new List<PropertiesValue>
                        {
                            new()
                            {
                                PropertyId=propertyId,
                                Value = mappedType,
                            },
                            new()
                            {
                                PropertyId=26,
                                Value = mappedType,
                            },
                        },
                    });

                }

                await SaveRecordsAsync("tasksSuccessRecords.txt", successRecords);
                await SaveRecordsAsync("tasksFailedRecords.txt", failedRecords);
                await SaveRecordsAsync("tasksDateFallbackRecords.txt", dateFallbackRecords);
                return TasksList;
            }
            catch (Exception e)
            {

            }
            return null;
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

        private async System.Threading.Tasks.Task<List<Data.TargetModels.Milestone>> PostMilestoneAsync(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate,
            Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            try
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

                var MilestonesList = new List<Data.TargetModels.Milestone>();
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
                        dateFallbackRecords.Add($"Project ID: {projectId}, Milestone: {milestone.Name}, Issue ID: {milestone.IssueId} , AssignedTo: Added default user.");
                    }

                    var destination = new Milestone()
                    {
                        Task = new Data.TargetModels.Task()
                        {
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = defaultUserId,
                            Title = milestone.Name,
                            ActualStartDate = startDate.DateTime,
                            StartDate = startDate.DateTime,
                            FinishDate = finishDate.DateTime,
                            Guid = Guid.NewGuid().ToString(),
                            LevelId = projectId,
                            LogId = 1,
                            TaskType = "Milestone",
                            PropertiesValues = new List<PropertiesValue>
                        {
                            new()
                            {
                                Id = 0,
                                PropertyId=_configuration.GetSection("TaskProperties:Type").GetValue<int>("PropertyId"),
                                Value = mappedType,
                            }
                        },
                        },
                        Attachments = "[]",
                        CompletionPercentage = progress,
                        Weight = 0.00001,
                        PlannedFinishDate = finishDate.DateTime,
                        PlannedStartDate = startDate.DateTime,
                        LevelId = projectId,
                        Name = milestone.Name,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = defaultUserId,
                        LogId = 3,
                        PropertiesValues = new List<PropertiesValue>
                        {
                            new()
                            {
                                Id = 0,
                                PropertyId=propertyId,
                                Value = mappedType,
                            },
                            new()
                            {
                                PropertyId = 28
                            }
                        },
                    };

                    MilestonesList.Add(destination);
                }

                await SaveRecordsAsync("milestonesSuccessRecords.txt", successRecords);
                await SaveRecordsAsync("milestonesFailedRecords.txt", failedRecords);
                await SaveRecordsAsync("milestonesDateFallbackRecords.txt", dateFallbackRecords);

                return MilestonesList;
            }
            catch (Exception ex)
            {
            }
            return new List<Milestone>();
        }

        private async System.Threading.Tasks.Task<List<Deliverable>> PostDeliverableAsync(long mtProjectId, int projectId, DateTime projectStartDate, DateTime projectFinishDate,
    Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
        {
            List<Deliverable> deliverableDetails = new List<Deliverable>();

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
                    dateFallbackRecords.Add($"Project ID: {projectId}, Deliverable: {deliverable.Title}, Deliverable ID: {deliverable.MtDeliverablId}, AssignedTo: Added default user.");
                }

                var deliverableEntity = new Deliverable()
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = defaultUserId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = defaultUserId,
                    Attachments = "[]",
                    LevelId = projectId,
                    CompletionPercentage = progress,
                    LogId = 2,
                    PropertiesValues = new List<PropertiesValue>
                    {
                        new PropertiesValue
                        {
                            PropertyId = propertyId,
                            Value = mappedType
                        },
                        new PropertiesValue
                        {
                            PropertyId = 27 
                        }
                    },
                    PlannedStartDate = plannedStartDate.DateTime,
                    PlannedFinishDate = plannedStartDate.DateTime,
                    Title = deliverable.Title,
                    Task = new Data.TargetModels.Task
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = defaultUserId,
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = defaultUserId,
                        StartDate = plannedStartDate.DateTime,
                        FinishDate = plannedFinishDate.DateTime,
                        Title = deliverable.Title,
                        Guid = Guid.NewGuid().ToString(),
                        LevelId = projectId,
                        LogId = 1, 
                        TaskType = "Deliverable"
                    },
                    MtDeliverablId = deliverable.MtDeliverablId.Value,
                    EarnedValue = deliverable.EarnedValue,
                    PaymentPlanStatus = deliverable.PaymentPlanStatus,
                    Amount = deliverable.Amount,
                    InvoiceNumber = deliverable.InvoiceNumber,
                };

                deliverableDetails.Add(deliverableEntity);
            }

            await SaveRecordsAsync("deliverablesSuccessRecords.txt", successRecords);
            await SaveRecordsAsync("deliverablesFailedRecords.txt", failedRecords);
            await SaveRecordsAsync("deliverablesDateFallbackRecords.txt", dateFallbackRecords);

            await _targetContext.Deliverables.AddRangeAsync(deliverableDetails);
            await _targetContext.SaveChangesAsync();
            return deliverableDetails;
        }
        private async System.Threading.Tasks.Task PostRiskAsync(long mtProjectId, int projectId, List<Deliverable> deliverableInfos, DateTime projectFinishDate, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
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

        private async System.Threading.Tasks.Task PostIssueAsync(long mtProjectId, int projectId, List<Deliverable> deliverableInfos, DateTime projectFinishDate, Dictionary<string, string> accountIdToEmailMap, List<UserInfo> users)
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
                    dateFallbackRecords.Add($"Project ID: {projectId}, Issue: {issue.Title}, Issue ID: {issue.IssueId}, DueDate: {dueDate}");
                }

                var assignedToEmail = issue.AssignedToAccountId != null && accountIdToEmailMap.ContainsKey(issue.AssignedToAccountId)
                    ? accountIdToEmailMap[issue.AssignedToAccountId]
                    : null;

                var assignedTo = assignedToEmail != null && userEmailToIdMap.ContainsKey(assignedToEmail)
                    ? userEmailToIdMap[assignedToEmail]
                    : defaultUserId;

                if (assignedTo == defaultUserId)
                {
                    dateFallbackRecords.Add($"Project ID: {projectId}, Issue: {issue.Title}, Issue ID: {issue.IssueId}, AssignedTo: Added default user.");
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
                    failedRecords.Add($"Project ID: {projectId}, Issue: {issue.Title}, Issue ID: {issue.IssueId}, Error: {errorResponse}");
                }
                else
                {
                    successRecords.Add($"Project ID: {projectId}, Issue: {issue.Title}");
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

        public async System.Threading.Tasks.Task ProcessDeliverablesAsync(int projectId, List<Deliverable> deliverables, string token)
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


        private async System.Threading.Tasks.Task CreateBoqAsync(int projectId, Deliverable deliverable)
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
                
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var boq = JsonDocument.Parse(responseContent).RootElement.GetProperty("data");
            deliverable.BoqId = boq.GetProperty("id").GetInt32();
        }

        private async System.Threading.Tasks.Task CreatePaymentPlanItemAsync(int projectId, int paymentPlanId, Deliverable deliverable)
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

        private async System.Threading.Tasks.Task CreateInvoiceAsync(int projectId, Deliverable deliverable, int paymentPlanItemId)
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

        private async System.Threading.Tasks.Task SubmitInvoiceAsync(int projectId, Deliverable deliverable)
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
