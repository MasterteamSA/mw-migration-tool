using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AspNetUserRole
    {
        public string UserId { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public int ModuleId { get; set; }

        public virtual AppRole Role { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
    }
}
