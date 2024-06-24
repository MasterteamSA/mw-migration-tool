using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class SchemaPhaseGateLog
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public int SchemaPhaseGateId { get; set; }

        public virtual Log Log { get; set; } = null!;
        public virtual SchemaPhaseGate SchemaPhaseGate { get; set; } = null!;
    }
}
