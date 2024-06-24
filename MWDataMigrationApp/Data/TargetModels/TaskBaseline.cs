using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class TaskBaseline
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Details { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string? Status { get; set; }
        public double Progress { get; set; }
        public string? Attachments { get; set; }
        public int LevelId { get; set; }
        public int Version { get; set; }
        public int? TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Task? Task { get; set; }
    }
}
