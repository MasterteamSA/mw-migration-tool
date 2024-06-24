using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class CategoryGroup
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}
