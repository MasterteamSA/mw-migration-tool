using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class TeamMember
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public int LevelId { get; set; }
        public string? RoleId { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
    }
}
