using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Level
    {
        public Level()
        {
            ConnectionSourceLevels = new HashSet<Connection>();
            ConnectionTargetLevels = new HashSet<Connection>();
            EscalationSchemas = new HashSet<EscalationSchema>();
            LevelsData = new HashSet<LevelsDatum>();
            LevelsLogs = new HashSet<LevelsLog>();
            ProcurementSchemas = new HashSet<ProcurementSchema>();
            PropertyLevels = new HashSet<Property>();
            PropertyRefs = new HashSet<Property>();
            PropertySections = new HashSet<PropertySection>();
            SchemaPhaseGates = new HashSet<SchemaPhaseGate>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Order { get; set; }
        public string Description { get; set; } = null!;
        public bool HasWorkSpace { get; set; }
        public string? Logs { get; set; }
        public string? Status { get; set; }
        public bool IsLeaf { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<Connection> ConnectionSourceLevels { get; set; }
        public virtual ICollection<Connection> ConnectionTargetLevels { get; set; }
        public virtual ICollection<EscalationSchema> EscalationSchemas { get; set; }
        public virtual ICollection<LevelsDatum> LevelsData { get; set; }
        public virtual ICollection<LevelsLog> LevelsLogs { get; set; }
        public virtual ICollection<ProcurementSchema> ProcurementSchemas { get; set; }
        public virtual ICollection<Property> PropertyLevels { get; set; }
        public virtual ICollection<Property> PropertyRefs { get; set; }
        public virtual ICollection<PropertySection> PropertySections { get; set; }
        public virtual ICollection<SchemaPhaseGate> SchemaPhaseGates { get; set; }
    }
}
