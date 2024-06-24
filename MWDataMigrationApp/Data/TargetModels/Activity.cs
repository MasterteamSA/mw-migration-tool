using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Activity
    {
        public Activity()
        {
            Actions = new HashSet<Action>();
        }

        public int Id { get; set; }
        public int RegistryId { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public bool IsActive { get; set; }
        public int WorkflowActivityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Registery Registry { get; set; } = null!;
        public virtual WorkflowActivity WorkflowActivity { get; set; } = null!;
        public virtual ICollection<Action> Actions { get; set; }
    }
}
