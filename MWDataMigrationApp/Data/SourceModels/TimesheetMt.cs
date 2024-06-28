using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class TimesheetMt
    {
        public int SeqId { get; set; }
        public string TempoWorklogId { get; set; }
        public int? IssueId { get; set; }
        public int? TimeSpentSeconds { get; set; }
        public int? BillableSeconds { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AccountId { get; set; }
        public int? Count { get; set; }
    }
}
