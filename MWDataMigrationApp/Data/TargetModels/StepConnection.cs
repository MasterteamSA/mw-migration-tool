using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class StepConnection
    {
        public int Id { get; set; }
        public int StepConnectionSchemaId { get; set; }
        public int SourceStepId { get; set; }
        public int TargetStepId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Step SourceStep { get; set; } = null!;
        public virtual StepConnectionsSchema StepConnectionSchema { get; set; } = null!;
        public virtual Step TargetStep { get; set; } = null!;
    }
}
