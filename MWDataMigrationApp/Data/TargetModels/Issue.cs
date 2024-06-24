using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Issue
    {
        public Issue()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
            Dependencies = new HashSet<Dependency>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Details { get; set; }
        public string? Category { get; set; }
        public string? Impact { get; set; }
        public string? Classification { get; set; }
        public string? ResolutionPlan { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? ClosedBy { get; set; }
        public bool HasEscalations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? Attachments { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }

        public virtual ICollection<Dependency> Dependencies { get; set; }
    }
}
