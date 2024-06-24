using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class SchemaPhaseGateItem
    {
        public SchemaPhaseGateItem()
        {
            PhaseGateItems = new HashSet<PhaseGateItem>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public bool IsMandatory { get; set; }
        public int SchemaPhaseGateId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual SchemaPhaseGate SchemaPhaseGate { get; set; } = null!;
        public virtual ICollection<PhaseGateItem> PhaseGateItems { get; set; }
    }
}
