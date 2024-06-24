using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class AppUser
    {
        public AppUser()
        {
            AppGroupUserAppUsers = new HashSet<AppGroupUser>();
            AppGroupUserUsers = new HashSet<AppGroupUser>();
            AppUserClaims = new HashSet<AppUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            DeviceTokens = new HashSet<DeviceToken>();
        }

        public string Id { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public int Stars { get; set; }
        public string Mobile { get; set; } = null!;
        public string Ext { get; set; } = null!;
        public bool IsExternal { get; set; }
        public string ExtraInfo { get; set; } = null!;
        public string ProviderExtraInfo { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<AppGroupUser> AppGroupUserAppUsers { get; set; }
        public virtual ICollection<AppGroupUser> AppGroupUserUsers { get; set; }
        public virtual ICollection<AppUserClaim> AppUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<DeviceToken> DeviceTokens { get; set; }
    }
}
