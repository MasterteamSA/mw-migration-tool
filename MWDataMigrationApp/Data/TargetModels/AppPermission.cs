using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppPermission
    {
        public AppPermission()
        {
            AppGroupPermissions = new HashSet<AppGroupPermission>();
            AppRolePermissions = new HashSet<AppRolePermission>();
        }

        public int Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public int? ModuleId { get; set; }
        public string ModuleType { get; set; } = null!;
        public string Command { get; set; } = null!;
        public string? AppRoleId { get; set; }

        public virtual AppRole? AppRole { get; set; }
        public virtual ICollection<AppGroupPermission> AppGroupPermissions { get; set; }
        public virtual ICollection<AppRolePermission> AppRolePermissions { get; set; }
    }
}
