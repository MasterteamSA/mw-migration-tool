using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class StepsSchema
    {
        public StepsSchema()
        {
            PropertiesSteps = new HashSet<PropertiesStep>();
            StepConnectionsSchemaSourceStepSchemas = new HashSet<StepConnectionsSchema>();
            StepConnectionsSchemaTargetStepSchemas = new HashSet<StepConnectionsSchema>();
            Steps = new HashSet<Step>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TargetType { get; set; }
        public string? TargetValue { get; set; }
        public int Sla { get; set; }
        public bool IsInitial { get; set; }
        public int RequestSchemaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual RequestsSchema RequestSchema { get; set; } = null!;
        public virtual ICollection<PropertiesStep> PropertiesSteps { get; set; }
        public virtual ICollection<StepConnectionsSchema> StepConnectionsSchemaSourceStepSchemas { get; set; }
        public virtual ICollection<StepConnectionsSchema> StepConnectionsSchemaTargetStepSchemas { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
    }
}
