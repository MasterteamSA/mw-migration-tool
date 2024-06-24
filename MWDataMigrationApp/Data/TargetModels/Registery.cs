using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Registery
    {
        public Registery()
        {
            Activities = new HashSet<Activity>();
            History1s = new HashSet<History1>();
        }

        public int Id { get; set; }
        public int WorkflowRegistryId { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Payload { get; set; }
        public bool IsDraft { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int? RefId { get; set; }

        public virtual WorkflowRegistry WorkflowRegistry { get; set; } = null!;
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<History1> History1s { get; set; }
    }
}
