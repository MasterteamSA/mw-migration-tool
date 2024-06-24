using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class LevelDataFinancial
    {
        public int Id { get; set; }
        public int LevelDataId { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum LevelData { get; set; } = null!;
    }
}
