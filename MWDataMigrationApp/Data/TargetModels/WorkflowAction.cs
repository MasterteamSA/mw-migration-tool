using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class WorkflowAction
    {
        public WorkflowAction()
        {
            Actions = new HashSet<Action>();
        }

        public int Id { get; set; }
        public int WorkflowActivityId { get; set; }
        public string? TargetValue { get; set; }
        public string TargetType { get; set; } = null!;
        public string GroupSelection { get; set; } = null!;
        public int Sla { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual WorkflowActivity WorkflowActivity { get; set; } = null!;
        public virtual ICollection<Action> Actions { get; set; }
    }
}
