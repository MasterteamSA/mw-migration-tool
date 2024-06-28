using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class ProjectDomainMt
    {
        public string UserAccountId { get; set; }
        public string UserName { get; set; }
        public long? ProjectId { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
    }
}
