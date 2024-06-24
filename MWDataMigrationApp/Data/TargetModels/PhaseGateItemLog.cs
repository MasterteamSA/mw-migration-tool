using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PhaseGateItemLog
    {
        public int Id { get; set; }
        public string Comment { get; set; } = null!;
        public string? Attachments { get; set; }
        public int PhaseGateItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual PhaseGateItem PhaseGateItem { get; set; } = null!;
    }
}
