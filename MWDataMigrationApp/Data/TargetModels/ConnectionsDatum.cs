using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ConnectionsDatum
    {
        public int Id { get; set; }
        public int TargetLevelDataId { get; set; }
        public int? SourceLevelDataId { get; set; }
        public int ConnectionId { get; set; }
        public double Weight { get; set; }

        public virtual Connection Connection { get; set; } = null!;
        public virtual LevelsDatum? SourceLevelData { get; set; }
        public virtual LevelsDatum TargetLevelData { get; set; } = null!;
    }
}
