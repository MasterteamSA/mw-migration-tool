using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class UserConnection
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? ConnectionId { get; set; }
        public string? RequestHeaders { get; set; }
        public string? Status { get; set; }
        public DateTime LastPing { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
