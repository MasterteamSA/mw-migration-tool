using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class ProjectsMt
    {
        public long? ProjectId { get; set; }
        public string? ProjectKey { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool? IsPrivate { get; set; }
        public string? LeadAccountId { get; set; }
        public string? LeadName { get; set; }
    }
}
