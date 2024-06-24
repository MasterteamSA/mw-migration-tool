using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class DiscussionLog
    {
        public int Id { get; set; }
        public string? Model { get; set; }
        public string? ModelType { get; set; }
        public int RecordId { get; set; }
        public string? Comment { get; set; }
        public string? Attachments { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
