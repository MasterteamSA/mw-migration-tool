using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class History
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int? StepId { get; set; }
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public string? FileUid { get; set; }
        public string ActionSource { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual Step? Step { get; set; }
    }
}
