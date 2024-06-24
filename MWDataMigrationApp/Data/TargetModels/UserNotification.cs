using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class UserNotification
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Details { get; set; }
        public string? Model { get; set; }
        public string? NotificationTo { get; set; }
        public string? CultureName { get; set; }
        public bool Seen { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
