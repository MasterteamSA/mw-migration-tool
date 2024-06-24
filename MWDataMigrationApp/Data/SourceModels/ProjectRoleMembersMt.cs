using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class ProjectRoleMembersMt
    {
        public long? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public long? ProjectRoleId { get; set; }
        public string? ProjectRoleName { get; set; }
        public string? UserAccountId { get; set; }
        public string? UserName { get; set; }
        public string? GroupName { get; set; }
    }
}
