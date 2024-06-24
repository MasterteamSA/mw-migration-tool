using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Closure
    {
        public Closure()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Attachments { get; set; }
        public string? Notes { get; set; }
        public string? LearnedLessons { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? ClosureReason { get; set; }
        public string? OtherDetails { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
    }
}
