using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Lookup
    {
        public Lookup()
        {
            LookupItems = new HashSet<LookupItem>();
        }

        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string? DisplayName { get; set; }
        public bool IsManageableByUser { get; set; }
        public bool IsCreatedByUser { get; set; }

        public virtual ICollection<LookupItem> LookupItems { get; set; }
    }
}
