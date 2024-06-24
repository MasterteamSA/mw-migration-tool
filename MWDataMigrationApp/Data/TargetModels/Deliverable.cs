using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Deliverable
    {
        public Deliverable()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
            TimeSheetItems = new HashSet<TimeSheetItem>();
            Expenses = new HashSet<Expense>();
            PaymentPlanItems = new HashSet<PaymentPlanItem>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime PlannedFinishDate { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public double CompletionPercentage { get; set; }
        public string? Status { get; set; }
        public int? DeliverableAcceptanceId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? Attachments { get; set; }
        public int? PhaseGateId { get; set; }

        public virtual DeliverableAcceptance? DeliverableAcceptance { get; set; }
        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual PhaseGate? PhaseGate { get; set; }
        public virtual Task Task { get; set; } = null!;
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
        public virtual ICollection<TimeSheetItem> TimeSheetItems { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<PaymentPlanItem> PaymentPlanItems { get; set; }
    }
}
