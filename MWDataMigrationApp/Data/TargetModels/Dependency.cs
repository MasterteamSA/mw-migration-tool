using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Dependency
    {
        public Dependency()
        {
            Issues = new HashSet<Issue>();
            Risks = new HashSet<Risk>();
        }

        public int Id { get; set; }
        public string? Owner { get; set; }
        public string? Category { get; set; }
        public string? Relationship { get; set; }
        public int AffectingTaskId { get; set; }
        public int AffectedTaskId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Task AffectedTask { get; set; } = null!;
        public virtual Task AffectingTask { get; set; } = null!;

        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<Risk> Risks { get; set; }
    }
}
