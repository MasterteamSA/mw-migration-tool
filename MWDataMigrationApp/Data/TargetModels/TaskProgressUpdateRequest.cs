using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class TaskProgressUpdateRequest
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public double Progress { get; set; }
        public string TaskType { get; set; } = null!;
        public DateTime SubittionDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public string? RequestedBy { get; set; }
        public string Status { get; set; } = null!;
        public string? Comments { get; set; }
        public string? Attachments { get; set; }
        public string? AssignedTo { get; set; }
        public int? LevelId { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDelegated { get; set; }
    }
}
