using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWDataMigrationApp.Data.SourceModels
{
    public class AdJiraUser
    {
        public string AD_DisplayName { get; set; }
        public string AD_UserPrincipalName { get; set; }
        public string AD_USER_NAME { get; set; }
        public string JIRA_EMAIL { get; set; }
        public string JIRA_NAME { get; set; }
        public string JIRA_USER_NAME { get; set; }
    }
}
