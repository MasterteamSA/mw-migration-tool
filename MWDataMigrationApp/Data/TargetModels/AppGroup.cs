using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppGroup
    {
        public AppGroup()
        {
            AppAccessibilityGroups = new HashSet<AppAccessibilityGroup>();
            AppGroupPermissions = new HashSet<AppGroupPermission>();
            AppGroupUsers = new HashSet<AppGroupUser>();
            InstanceGroupPermissions = new HashSet<InstanceGroupPermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActivated { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<AppAccessibilityGroup> AppAccessibilityGroups { get; set; }
        public virtual ICollection<AppGroupPermission> AppGroupPermissions { get; set; }
        public virtual ICollection<AppGroupUser> AppGroupUsers { get; set; }
        public virtual ICollection<InstanceGroupPermission> InstanceGroupPermissions { get; set; }
    }
}
