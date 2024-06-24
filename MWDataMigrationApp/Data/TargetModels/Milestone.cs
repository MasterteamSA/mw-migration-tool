using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Milestone
    {
        public Milestone()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime PlannedFinishDate { get; set; }
        public double Weight { get; set; }
        public double CompletionPercentage { get; set; }
        public string? Status { get; set; }
        public int TaskId { get; set; }
        public int? LevelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? Attachments { get; set; }
        public int? PhaseGateId { get; set; }
        public DateTime PlannedStartDate { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual LevelsDatum? LevelData { get; set; }
        public virtual Log Log { get; set; } = null!;
        public virtual PhaseGate? PhaseGate { get; set; }
        public virtual Task Task { get; set; } = null!;
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
    }
}
