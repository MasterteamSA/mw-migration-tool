using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class History1
    {
        public int Id { get; set; }
        public int RegistryId { get; set; }
        public int? ActionId { get; set; }
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public string? FileUid { get; set; }
        public int ActionSource { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Action? Action { get; set; }
        public virtual Registery Registry { get; set; } = null!;
    }
}
