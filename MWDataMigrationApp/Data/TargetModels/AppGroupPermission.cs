using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppGroupPermission
    {
        public int Id { get; set; }
        public int AppPermissionId { get; set; }
        public int GroupId { get; set; }

        public virtual AppPermission AppPermission { get; set; } = null!;
        public virtual AppGroup Group { get; set; } = null!;
    }
}
