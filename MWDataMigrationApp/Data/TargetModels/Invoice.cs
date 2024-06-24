using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Invoice
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public double Amount { get; set; }
        public double RemainingAmount { get; set; }
        public string? Attachments { get; set; }
        public string? Status { get; set; }
        public int PaymentPlanItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual PaymentPlanItem PaymentPlanItem { get; set; } = null!;
    }
}
