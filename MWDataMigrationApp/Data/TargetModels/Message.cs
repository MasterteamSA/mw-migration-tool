using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Message
    {
        public long Id { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Sender { get; set; }
        public int Group { get; set; }
        public string Type { get; set; } = null!;
        public string? DownloadUrl { get; set; }
    }
}
