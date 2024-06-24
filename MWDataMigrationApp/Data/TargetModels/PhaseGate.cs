using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PhaseGate
    {
        public PhaseGate()
        {
            Deliverables = new HashSet<Deliverable>();
            Milestones = new HashSet<Milestone>();
            PhaseGateItems = new HashSet<PhaseGateItem>();
            PropertiesValues = new HashSet<PropertiesValue>();
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsCompleted { get; set; }
        public int LevelId { get; set; }
        public int SchemaPhaseGateId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Attachments { get; set; }
        public int LogId { get; set; }
        public int Order { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual SchemaPhaseGate SchemaPhaseGate { get; set; } = null!;
        public virtual ICollection<Deliverable> Deliverables { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
        public virtual ICollection<PhaseGateItem> PhaseGateItems { get; set; }
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
