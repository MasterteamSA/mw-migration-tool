using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class EscalationAction
    {
        public int Id { get; set; }
        public int EscalationId { get; set; }
        public string? Target { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? Notes { get; set; }
        public string? Attachments { get; set; }
        public string? Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Escalation Escalation { get; set; } = null!;
    }
}
