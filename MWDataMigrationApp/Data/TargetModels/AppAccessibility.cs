using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppAccessibility
    {
        public AppAccessibility()
        {
            AppAccessibilityGroups = new HashSet<AppAccessibilityGroup>();
        }

        public int Id { get; set; }
        public string ModuleKey { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<AppAccessibilityGroup> AppAccessibilityGroups { get; set; }
    }
}
