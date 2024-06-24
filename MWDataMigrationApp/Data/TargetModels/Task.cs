using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Task
    {
        public Task()
        {
            DependencyAffectedTasks = new HashSet<Dependency>();
            DependencyAffectingTasks = new HashSet<Dependency>();
            PredecessorPredecessorTasks = new HashSet<Predecessor>();
            PredecessorTasks = new HashSet<Predecessor>();
            PropertiesValues = new HashSet<PropertiesValue>();
            TaskBaselines = new HashSet<TaskBaseline>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Details { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public double Progress { get; set; }
        public string? Attachments { get; set; }
        public int LevelId { get; set; }
        public bool IsMilestone { get; set; }
        public bool IsSummary { get; set; }
        public string? Guid { get; set; }
        public string? ParentGuid { get; set; }
        public string TaskType { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LogId { get; set; }
        public int? PhaseGateId { get; set; }
        public bool? HasEscalations { get; set; }
        public int Order { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual PhaseGate? PhaseGate { get; set; }
        public virtual Deliverable? Deliverable { get; set; }
        public virtual Milestone? Milestone { get; set; }
        public virtual ICollection<Dependency> DependencyAffectedTasks { get; set; }
        public virtual ICollection<Dependency> DependencyAffectingTasks { get; set; }
        public virtual ICollection<Predecessor> PredecessorPredecessorTasks { get; set; }
        public virtual ICollection<Predecessor> PredecessorTasks { get; set; }
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
        public virtual ICollection<TaskBaseline> TaskBaselines { get; set; }
    }
}
