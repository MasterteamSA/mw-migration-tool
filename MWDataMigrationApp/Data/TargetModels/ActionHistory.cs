using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ActionHistory
    {
        public int Id { get; set; }
        public int InstanceId { get; set; }
        public string? Model { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModelType { get; set; }
        public string? Event { get; set; }
        public string? CreatedBy { get; set; }
        public string? Payload { get; set; }
        public string? Action { get; set; }
    }
}
