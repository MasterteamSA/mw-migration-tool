using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class SchemaPhaseGate
    {
        public SchemaPhaseGate()
        {
            PhaseGates = new HashSet<PhaseGate>();
            SchemaPhaseGateItems = new HashSet<SchemaPhaseGateItem>();
            SchemaPhaseGateLogs = new HashSet<SchemaPhaseGateLog>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Order { get; set; }
        public string? Icon { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Weight { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual ICollection<PhaseGate> PhaseGates { get; set; }
        public virtual ICollection<SchemaPhaseGateItem> SchemaPhaseGateItems { get; set; }
        public virtual ICollection<SchemaPhaseGateLog> SchemaPhaseGateLogs { get; set; }
    }
}
