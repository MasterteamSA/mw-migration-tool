using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppCredential
    {
        public string AppId { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string AppName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string OwnerEntity { get; set; } = null!;
    }
}
