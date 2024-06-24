using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PropertiesValue
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public int PropertyId { get; set; }
        public int? LevelDataId { get; set; }
        public int? LogDataId { get; set; }
        public int? Task { get; set; }
        public int? Deliverable { get; set; }
        public int? Milestone { get; set; }
        public int? Risk { get; set; }
        public int? Issue { get; set; }
        public int? Stakeholder { get; set; }
        public int? Charter { get; set; }
        public int? Closure { get; set; }
        public int? DeliverableAcceptance { get; set; }
        public int? ChangeRequest { get; set; }
        public int? ProcurementStage { get; set; }
        public int? ProcurementStageId { get; set; }
        public int? PhaseGateId { get; set; }
        public int? GeneralTask { get; set; }

        public virtual ChangeRequest? ChangeRequestNavigation { get; set; }
        public virtual Charter? CharterNavigation { get; set; }
        public virtual Closure? ClosureNavigation { get; set; }
        public virtual DeliverableAcceptance? DeliverableAcceptanceNavigation { get; set; }
        public virtual Deliverable? DeliverableNavigation { get; set; }
        public virtual GeneralTask? GeneralTaskNavigation { get; set; }
        public virtual Issue? IssueNavigation { get; set; }
        public virtual LevelsDatum? LevelData { get; set; }
        public virtual LogsDatum? LogData { get; set; }
        public virtual Milestone? MilestoneNavigation { get; set; }
        public virtual PhaseGate? PhaseGate { get; set; }
        public virtual ProcurementStage? ProcurementStageNavigation { get; set; }
        public virtual Property Property { get; set; } = null!;
        public virtual Risk? RiskNavigation { get; set; }
        public virtual Stakeholder? StakeholderNavigation { get; set; }
        public virtual Task? TaskNavigation { get; set; }
    }
}
