using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PaymentPlanItemBoq
    {
        public int Id { get; set; }
        public int PaymentPlanItemId { get; set; }
        public int BoqId { get; set; }
        public int AllocatedQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual BillOfQuantity Boq { get; set; } = null!;
        public virtual PaymentPlanItem PaymentPlanItem { get; set; } = null!;
    }
}
