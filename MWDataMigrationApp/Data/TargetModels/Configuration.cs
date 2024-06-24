using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string? Model { get; set; }
        public string? Value { get; set; }
        public string? Property { get; set; }
    }
}
