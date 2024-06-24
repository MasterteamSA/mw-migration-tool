using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Cache
    {
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}
