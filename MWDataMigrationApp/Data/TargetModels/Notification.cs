using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string UserId { get; set; } = null!;
        public string Links { get; set; } = null!;
        public string UserCausedEvent { get; set; } = null!;
    }
}
