using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppRolePermission
    {
        public int Id { get; set; }
        public int AppPermissionId { get; set; }
        public string RoleId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AppPermission AppPermission { get; set; } = null!;
        public virtual AppRole Role { get; set; } = null!;
    }
}
