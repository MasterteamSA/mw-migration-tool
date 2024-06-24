using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class EscalationSchema
    {
        public EscalationSchema()
        {
            Escalations = new HashSet<Escalation>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Target { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
        public bool IsCurrent { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual ICollection<Escalation> Escalations { get; set; }
    }
}
