using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class InstanceGroupPermission
    {
        public int Id { get; set; }
        public int InstancePermissionId { get; set; }
        public int GroupId { get; set; }

        public virtual AppGroup Group { get; set; } = null!;
    }
}
