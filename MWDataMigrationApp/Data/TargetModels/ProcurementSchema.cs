using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ProcurementSchema
    {
        public ProcurementSchema()
        {
            ProcurementStages = new HashSet<ProcurementStage>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Order { get; set; }
        public int LevelId { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual ICollection<ProcurementStage> ProcurementStages { get; set; }
    }
}
