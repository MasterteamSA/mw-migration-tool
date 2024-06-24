using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Log
    {
        public Log()
        {
            BasicProperties = new HashSet<BasicProperty>();
            ChangeRequests = new HashSet<ChangeRequest>();
            Charters = new HashSet<Charter>();
            Closures = new HashSet<Closure>();
            DeliverableAcceptances = new HashSet<DeliverableAcceptance>();
            Deliverables = new HashSet<Deliverable>();
            Issues = new HashSet<Issue>();
            LevelsLogs = new HashSet<LevelsLog>();
            LogsData = new HashSet<LogsDatum>();
            Milestones = new HashSet<Milestone>();
            PhaseGates = new HashSet<PhaseGate>();
            ProcurementStages = new HashSet<ProcurementStage>();
            Properties = new HashSet<Property>();
            Risks = new HashSet<Risk>();
            SchemaPhaseGateLogs = new HashSet<SchemaPhaseGateLog>();
            Stakeholders = new HashSet<Stakeholder>();
            Tasks = new HashSet<Task>();
            TimeSheets = new HashSet<TimeSheet>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        public string? Description { get; set; }
        public bool CanBeProperty { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? CanHaveProperties { get; set; }

        public virtual ICollection<BasicProperty> BasicProperties { get; set; }
        public virtual ICollection<ChangeRequest> ChangeRequests { get; set; }
        public virtual ICollection<Charter> Charters { get; set; }
        public virtual ICollection<Closure> Closures { get; set; }
        public virtual ICollection<DeliverableAcceptance> DeliverableAcceptances { get; set; }
        public virtual ICollection<Deliverable> Deliverables { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<LevelsLog> LevelsLogs { get; set; }
        public virtual ICollection<LogsDatum> LogsData { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
        public virtual ICollection<PhaseGate> PhaseGates { get; set; }
        public virtual ICollection<ProcurementStage> ProcurementStages { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Risk> Risks { get; set; }
        public virtual ICollection<SchemaPhaseGateLog> SchemaPhaseGateLogs { get; set; }
        public virtual ICollection<Stakeholder> Stakeholders { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<TimeSheet> TimeSheets { get; set; }
    }
}
