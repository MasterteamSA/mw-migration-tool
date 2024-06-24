using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class BillOfQuantity
    {
        public BillOfQuantity()
        {
            PaymentPlanItemBoqs = new HashSet<PaymentPlanItemBoq>();
        }

        public int Id { get; set; }
        public string? ItemName { get; set; }
        public int TotalQuantity { get; set; }
        public double ItemPrice { get; set; }
        public double TotalAmount { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual ICollection<PaymentPlanItemBoq> PaymentPlanItemBoqs { get; set; }
    }
}
