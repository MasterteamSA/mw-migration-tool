using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class BasicProperty
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string ViewType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public bool IsTranslatable { get; set; }
        public int Order { get; set; }
        public string? Configuration { get; set; }
        public int LogId { get; set; }
        public bool? IsCalculated { get; set; }

        public virtual Log Log { get; set; } = null!;
    }
}
