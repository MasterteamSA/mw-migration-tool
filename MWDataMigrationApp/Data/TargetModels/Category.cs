using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Category
    {
        public Category()
        {
            CategoryGroups = new HashSet<CategoryGroup>();
            CategoryLevels = new HashSet<CategoryLevel>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SelectedSchemas { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<CategoryGroup> CategoryGroups { get; set; }
        public virtual ICollection<CategoryLevel> CategoryLevels { get; set; }
    }
}
