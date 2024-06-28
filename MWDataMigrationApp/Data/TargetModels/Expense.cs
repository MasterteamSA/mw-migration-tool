using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Expense
    {
        public Expense()
        {
            Deliverables = new HashSet<Deliverable>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public string? Type { get; set; }
        public DateTime? Date { get; set; }
        public string Category { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;

        public virtual ICollection<Deliverable> Deliverables { get; set; }
    }
}
