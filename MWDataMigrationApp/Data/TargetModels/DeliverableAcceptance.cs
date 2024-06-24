using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class DeliverableAcceptance
    {
        public DeliverableAcceptance()
        {
            Deliverables = new HashSet<Deliverable>();
            PropertiesValues = new HashSet<PropertiesValue>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? Attachments { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ICollection<Deliverable> Deliverables { get; set; }
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
    }
}
