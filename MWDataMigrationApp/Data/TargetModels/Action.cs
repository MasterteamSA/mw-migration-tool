using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Action
    {
        public Action()
        {
            History1s = new HashSet<History1>();
        }

        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int WorkflowActionId { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Target { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int Sla { get; set; }

        public virtual Activity Activity { get; set; } = null!;
        public virtual WorkflowAction WorkflowAction { get; set; } = null!;
        public virtual ICollection<History1> History1s { get; set; }
    }
}
