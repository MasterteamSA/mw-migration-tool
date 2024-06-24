using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PaymentPlanItem
    {
        public PaymentPlanItem()
        {
            Invoices = new HashSet<Invoice>();
            PaymentPlanItemBoqs = new HashSet<PaymentPlanItemBoq>();
            Deliverables = new HashSet<Deliverable>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DueDate { get; set; }
        public int PaymentPlanId { get; set; }
        public int? PaymentPlanId1 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalCost { get; set; }

        public virtual PaymentPlan PaymentPlan { get; set; } = null!;
        public virtual PaymentPlan? PaymentPlanId1Navigation { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<PaymentPlanItemBoq> PaymentPlanItemBoqs { get; set; }

        public virtual ICollection<Deliverable> Deliverables { get; set; }
    }
}
