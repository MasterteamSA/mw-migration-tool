using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PhaseGateItem
    {
        public PhaseGateItem()
        {
            PhaseGateItemLogs = new HashSet<PhaseGateItemLog>();
        }

        public int Id { get; set; }
        public bool IsCompleted { get; set; }
        public string? CompletedBy { get; set; }
        public int Status { get; set; }
        public int SchemaPhaseGateItemId { get; set; }
        public int PhaseGateId { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual PhaseGate PhaseGate { get; set; } = null!;
        public virtual SchemaPhaseGateItem SchemaPhaseGateItem { get; set; } = null!;
        public virtual ICollection<PhaseGateItemLog> PhaseGateItemLogs { get; set; }
    }
}
