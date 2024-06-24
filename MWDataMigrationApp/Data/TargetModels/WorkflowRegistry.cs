using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class WorkflowRegistry
    {
        public WorkflowRegistry()
        {
            Drafts = new HashSet<Draft>();
            Registeries = new HashSet<Registery>();
            WorkflowActivities = new HashSet<WorkflowActivity>();
        }

        public int Id { get; set; }
        public string? CommandName { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? PreventMultiInstances { get; set; }
        public string? InstanceReferenceProperty { get; set; }
        public bool? IsDraftActive { get; set; }

        public virtual ICollection<Draft> Drafts { get; set; }
        public virtual ICollection<Registery> Registeries { get; set; }
        public virtual ICollection<WorkflowActivity> WorkflowActivities { get; set; }
    }
}
