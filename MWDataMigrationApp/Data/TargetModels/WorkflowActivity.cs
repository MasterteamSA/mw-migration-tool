using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class WorkflowActivity
    {
        public WorkflowActivity()
        {
            Activities = new HashSet<Activity>();
            InversePreviousWorkflowActivity = new HashSet<WorkflowActivity>();
            WorkflowActions = new HashSet<WorkflowAction>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int WorkflowRegistryId { get; set; }
        public string Discriminator { get; set; } = null!;
        public string? Formula { get; set; }
        public string? FormulaRaw { get; set; }
        public int? TrueDirectionActivityId { get; set; }
        public int? FalseDirectionActivityId { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int? PreviousWorkflowActivityId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual WorkflowActivity? FalseDirectionActivity { get; set; }
        public virtual WorkflowActivity? PreviousWorkflowActivity { get; set; }
        public virtual WorkflowActivity? TrueDirectionActivity { get; set; }
        public virtual WorkflowRegistry WorkflowRegistry { get; set; } = null!;
        public virtual WorkflowActivity? InverseFalseDirectionActivity { get; set; }
        public virtual WorkflowActivity? InverseTrueDirectionActivity { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<WorkflowActivity> InversePreviousWorkflowActivity { get; set; }
        public virtual ICollection<WorkflowAction> WorkflowActions { get; set; }
    }
}
