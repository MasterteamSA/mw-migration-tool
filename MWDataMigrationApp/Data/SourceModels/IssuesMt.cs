using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class IssuesMt
    {
        public long? IssueId { get; set; }
        public string IssueKey { get; set; }
        public long? IssueTypeId { get; set; }
        public string IssueTypeName { get; set; }
        public long? IssueStatusId { get; set; }
        public string IssueStatusName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectKey { get; set; }
        public string Priority { get; set; }
        public long? Watchers { get; set; }
        public string Resolution { get; set; }
        public string CurrentAssigneeAccountId { get; set; }
        public string CurrentAssigneeName { get; set; }
        public string CreatorAccountId { get; set; }
        public string CreatorName { get; set; }
        public string ReporterAccountId { get; set; }
        public string ReporterName { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset? ResolutionDate { get; set; }
        public long? ParentIssueId { get; set; }
        public string ParentIssueKey { get; set; }
        public string ParentIssueSummary { get; set; }
        public long? ParentIssueTypeId { get; set; }
        public string ParentIssueTypeName { get; set; }
        public string ParentPriority { get; set; }
        public long? ParentIssueStatusId { get; set; }
        public string ParentIssueStatusName { get; set; }
        public DateTimeOffset? ActualEndDate { get; set; }
        public DateTimeOffset? ActualStartDate { get; set; }
        public DateTimeOffset? BaselineEndDate { get; set; }
        public DateTimeOffset? BaselineStartDate { get; set; }
        public string Budget10052 { get; set; }
        public double? EarnedValue { get; set; }
        public string Email10050 { get; set; }
        public DateTimeOffset? EndDate10097 { get; set; }
        public string EpicName10011 { get; set; }
        public string EpicStatus10012 { get; set; }
        public string Impact10004 { get; set; }
        public string InherentImpact10082 { get; set; }
        public string InherentProbability10084 { get; set; }
        public string InherentRisk10086 { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTimeOffset? NewExpectedDate { get; set; }
        public string Owner10054 { get; set; }
        public string PaymentPlan { get; set; }
        public DateTimeOffset? PlannedEndDate { get; set; }
        public DateTimeOffset? PlannedStartDate { get; set; }
        public double? PlannedValue { get; set; }
        public DateTimeOffset? StartDate10015 { get; set; }
        public long? TempoTeamId { get; set; }
        public string Type10074 { get; set; }
        public double? Value10071 { get; set; }
    }
}
