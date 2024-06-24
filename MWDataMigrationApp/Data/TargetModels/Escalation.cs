using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Escalation
    {
        public Escalation()
        {
            EscalationActions = new HashSet<EscalationAction>();
        }

        public int Id { get; set; }
        public int EscalationSchemaId { get; set; }
        public string? Model { get; set; }
        public int RefId { get; set; }
        public string? Description { get; set; }
        public string? Attachments { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public string? ApprovalStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual EscalationSchema EscalationSchema { get; set; } = null!;
        public virtual ICollection<EscalationAction> EscalationActions { get; set; }
    }
}
