using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class UsersMt
    {
        public string UserAccountId { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public bool? IsActive { get; set; }
        public string Email { get; set; }
        public long? TempoWorkloadId { get; set; }
    }
}
