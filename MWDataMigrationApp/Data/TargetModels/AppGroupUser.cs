using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppGroupUser
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AppUserId { get; set; }

        public virtual AppUser? AppUser { get; set; }
        public virtual AppGroup Group { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
    }
}
