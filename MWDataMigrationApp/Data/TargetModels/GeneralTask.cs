using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class GeneralTask
    {
        public GeneralTask()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Details { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string? Status { get; set; }
        public double Progress { get; set; }
        public string? Attachments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Attendees { get; set; }
        public string? Feedback { get; set; }

        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
    }
}
