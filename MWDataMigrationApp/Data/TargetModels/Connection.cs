using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Connection
    {
        public Connection()
        {
            ConnectionsData = new HashSet<ConnectionsDatum>();
        }

        public int Id { get; set; }
        public int? SourceLevelId { get; set; }
        public int TargetLevelId { get; set; }
        public bool IsOptional { get; set; }
        public bool SupportWeights { get; set; }
        public bool? AllowManyToMany { get; set; }

        public virtual Level? SourceLevel { get; set; }
        public virtual Level TargetLevel { get; set; } = null!;
        public virtual ICollection<ConnectionsDatum> ConnectionsData { get; set; }
    }
}
