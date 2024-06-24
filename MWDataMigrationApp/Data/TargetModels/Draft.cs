using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Draft
    {
        public int Id { get; set; }
        public int WorkflowRegistryId { get; set; }
        public string? Title { get; set; }
        public string? Payload { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual WorkflowRegistry WorkflowRegistry { get; set; } = null!;
    }
}
