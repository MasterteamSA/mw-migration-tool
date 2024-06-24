using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Predecessor
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int PredecessorTaskId { get; set; }
        public string PredecessorType { get; set; } = null!;
        public int PredecessorOffset { get; set; }
        public string? PredecessorOffsetType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Task PredecessorTask { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
