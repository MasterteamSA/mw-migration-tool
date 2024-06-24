using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class TimeSheet
    {
        public TimeSheet()
        {
            TimeSheetItems = new HashSet<TimeSheetItem>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime Day { get; set; }
        public int Status { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string? Desciption { get; set; }
        public int Hours { get; set; }
        public double HourRateSnapshot { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }
        public string? Attachments { get; set; }
        public string? Notes { get; set; }

        public virtual LevelsDatum Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ICollection<TimeSheetItem> TimeSheetItems { get; set; }
    }
}
