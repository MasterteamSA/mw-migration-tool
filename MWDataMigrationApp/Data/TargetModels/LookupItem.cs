using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class LookupItem
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public int LookupId { get; set; }
        public bool? AllowDelete { get; set; }

        public virtual Lookup Lookup { get; set; } = null!;
    }
}
