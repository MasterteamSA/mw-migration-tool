using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PaymentPlan
    {
        public PaymentPlan()
        {
            PaymentPlanItemPaymentPlanId1Navigations = new HashSet<PaymentPlanItem>();
            PaymentPlanItemPaymentPlans = new HashSet<PaymentPlanItem>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual ICollection<PaymentPlanItem> PaymentPlanItemPaymentPlanId1Navigations { get; set; }
        public virtual ICollection<PaymentPlanItem> PaymentPlanItemPaymentPlans { get; set; }
    }
}
