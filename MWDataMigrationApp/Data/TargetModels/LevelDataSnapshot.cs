using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class LevelDataSnapshot
    {
        public int Id { get; set; }
        public int LevelDataId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Budget { get; set; }
        public string? Spent { get; set; }
        public string? Remaining { get; set; }
        public string? ActulaProgress { get; set; }
        public string? PlannedProgress { get; set; }
        public string? BaselineProgress { get; set; }
        public string? Status { get; set; }
        public string? FinishDate { get; set; }
        public string? LevelName { get; set; }
        public string? Manager { get; set; }
        public string? StartDate { get; set; }

        public virtual LevelsDatum LevelData { get; set; } = null!;
    }
}
