using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppUserClaim
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string ClaimValueRef { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AppUser User { get; set; } = null!;
    }
}
