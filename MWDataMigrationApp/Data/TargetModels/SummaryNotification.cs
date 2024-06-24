using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class SummaryNotification
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public string UserId { get; set; } = null!;
    }
}
