using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ProgressStatus
    {
        public int Id { get; set; }
        public string PreviousActivities { get; set; } = null!;
        public string NextActivities { get; set; } = null!;
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
    }
}
