using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class ProjectsMt
    {
        public long? ProjectId { get; set; }
        public string ProjectKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ProjectContractValue { get; set; }
        public string Company { get; set; }
        public string BusninessLine { get; set; }
        public string Organization { get; set; }
        public string AccountManager { get; set; }
        public string LeadName { get; set; }
        public string PipeDriveLink { get; set; }
    }
}
