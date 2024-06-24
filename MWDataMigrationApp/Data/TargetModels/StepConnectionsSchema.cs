using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class StepConnectionsSchema
    {
        public StepConnectionsSchema()
        {
            StepConnections = new HashSet<StepConnection>();
        }

        public int Id { get; set; }
        public string? Formula { get; set; }
        public string? FormulaRaw { get; set; }
        public int Priority { get; set; }
        public int SourceStepSchemaId { get; set; }
        public int TargetStepSchemaId { get; set; }
        public int RequestSchemaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual RequestsSchema RequestSchema { get; set; } = null!;
        public virtual StepsSchema SourceStepSchema { get; set; } = null!;
        public virtual StepsSchema TargetStepSchema { get; set; } = null!;
        public virtual ICollection<StepConnection> StepConnections { get; set; }
    }
}
