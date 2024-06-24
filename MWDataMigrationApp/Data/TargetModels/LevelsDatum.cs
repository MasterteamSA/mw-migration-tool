using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class LevelsDatum
    {
        public LevelsDatum()
        {
            BillOfQuantities = new HashSet<BillOfQuantity>();
            CategoryLevels = new HashSet<CategoryLevel>();
            ChangeRequests = new HashSet<ChangeRequest>();
            Charters = new HashSet<Charter>();
            Closures = new HashSet<Closure>();
            ConnectionsDatumSourceLevelData = new HashSet<ConnectionsDatum>();
            ConnectionsDatumTargetLevelData = new HashSet<ConnectionsDatum>();
            DeliverableAcceptances = new HashSet<DeliverableAcceptance>();
            Deliverables = new HashSet<Deliverable>();
            Expenses = new HashSet<Expense>();
            Issues = new HashSet<Issue>();
            LevelDataFinancials = new HashSet<LevelDataFinancial>();
            LevelDataSnapshots = new HashSet<LevelDataSnapshot>();
            LogsData = new HashSet<LogsDatum>();
            MilestoneLevelData = new HashSet<Milestone>();
            MilestoneLevels = new HashSet<Milestone>();
            PaymentPlans = new HashSet<PaymentPlan>();
            PhaseGateItems = new HashSet<PhaseGateItem>();
            PhaseGates = new HashSet<PhaseGate>();
            ProcurementStages = new HashSet<ProcurementStage>();
            ProgressStatuses = new HashSet<ProgressStatus>();
            PropertiesValues = new HashSet<PropertiesValue>();
            Risks = new HashSet<Risk>();
            Stakeholders = new HashSet<Stakeholder>();
            TaskBaselines = new HashSet<TaskBaseline>();
            Tasks = new HashSet<Task>();
            TeamMembers = new HashSet<TeamMember>();
            TimeSheets = new HashSet<TimeSheet>();
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public bool IsActive { get; set; }
        public string? Attachments { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual ICollection<BillOfQuantity> BillOfQuantities { get; set; }
        public virtual ICollection<CategoryLevel> CategoryLevels { get; set; }
        public virtual ICollection<ChangeRequest> ChangeRequests { get; set; }
        public virtual ICollection<Charter> Charters { get; set; }
        public virtual ICollection<Closure> Closures { get; set; }
        public virtual ICollection<ConnectionsDatum> ConnectionsDatumSourceLevelData { get; set; }
        public virtual ICollection<ConnectionsDatum> ConnectionsDatumTargetLevelData { get; set; }
        public virtual ICollection<DeliverableAcceptance> DeliverableAcceptances { get; set; }
        public virtual ICollection<Deliverable> Deliverables { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<LevelDataFinancial> LevelDataFinancials { get; set; }
        public virtual ICollection<LevelDataSnapshot> LevelDataSnapshots { get; set; }
        public virtual ICollection<LogsDatum> LogsData { get; set; }
        public virtual ICollection<Milestone> MilestoneLevelData { get; set; }
        public virtual ICollection<Milestone> MilestoneLevels { get; set; }
        public virtual ICollection<PaymentPlan> PaymentPlans { get; set; }
        public virtual ICollection<PhaseGateItem> PhaseGateItems { get; set; }
        public virtual ICollection<PhaseGate> PhaseGates { get; set; }
        public virtual ICollection<ProcurementStage> ProcurementStages { get; set; }
        public virtual ICollection<ProgressStatus> ProgressStatuses { get; set; }
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
        public virtual ICollection<Risk> Risks { get; set; }
        public virtual ICollection<Stakeholder> Stakeholders { get; set; }
        public virtual ICollection<TaskBaseline> TaskBaselines { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
        public virtual ICollection<TimeSheet> TimeSheets { get; set; }
    }
}
