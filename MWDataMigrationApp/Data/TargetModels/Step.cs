using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Step
    {
        public Step()
        {
            RequestPropertyValues = new HashSet<RequestPropertyValue>();
            StepConnectionSourceSteps = new HashSet<StepConnection>();
            StepConnectionTargetSteps = new HashSet<StepConnection>();
        }

        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? TargetUsername { get; set; }
        public int StepSchemaId { get; set; }
        public int RequestId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ActionDate { get; set; }
        public string? ActionUser { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual StepsSchema StepSchema { get; set; } = null!;
        public virtual History? History { get; set; }
        public virtual ICollection<RequestPropertyValue> RequestPropertyValues { get; set; }
        public virtual ICollection<StepConnection> StepConnectionSourceSteps { get; set; }
        public virtual ICollection<StepConnection> StepConnectionTargetSteps { get; set; }
    }
}
