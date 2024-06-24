using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppRole
    {
        public AppRole()
        {
            AppPermissions = new HashSet<AppPermission>();
            AppRolePermissions = new HashSet<AppRolePermission>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
        }

        public string Id { get; set; } = null!;
        public int? ModuleId { get; set; }
        public int? SourceId { get; set; }
        public string ExtraInfo { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }

        public virtual ICollection<AppPermission> AppPermissions { get; set; }
        public virtual ICollection<AppRolePermission> AppRolePermissions { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
