using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class RequestPropertyValue
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public string? Metadata { get; set; }
        public int RequestPropertyId { get; set; }
        public int? StepId { get; set; }
        public int RequestId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual RequestProperty RequestProperty { get; set; } = null!;
        public virtual Step? Step { get; set; }
    }
}
