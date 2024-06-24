using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ProcurementStage
    {
        public ProcurementStage()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
        }

        public int Id { get; set; }
        public int ProcurementSchemaId { get; set; }
        public string? Name { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedFinishDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? Attachments { get; set; }
        public int LogId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsCurrent { get; set; }
        public int Order { get; set; }
        public int LevelId { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ProcurementSchema ProcurementSchema { get; set; } = null!;
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
    }
}
