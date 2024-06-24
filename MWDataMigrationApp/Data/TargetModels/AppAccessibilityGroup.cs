using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppAccessibilityGroup
    {
        public int Id { get; set; }
        public int AppAccessibilityId { get; set; }
        public int AppGroupId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual AppAccessibility AppAccessibility { get; set; } = null!;
        public virtual AppGroup AppGroup { get; set; } = null!;
    }
}
