using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PropertiesStep
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public int PropertyId { get; set; }
        public int StepSchemaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual RequestProperty Property { get; set; } = null!;
        public virtual StepsSchema StepSchema { get; set; } = null!;
    }
}
