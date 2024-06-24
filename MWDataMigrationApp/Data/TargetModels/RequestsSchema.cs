using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class RequestsSchema
    {
        public RequestsSchema()
        {
            RequestProperties = new HashSet<RequestProperty>();
            Requests = new HashSet<Request>();
            StepConnectionsSchemas = new HashSet<StepConnectionsSchema>();
            StepsSchemas = new HashSet<StepsSchema>();
        }

        public int Id { get; set; }
        public string? CommandName { get; set; }
        public string? DisplayName { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleType { get; set; }
        public bool IsValid { get; set; }
        public bool IsPublished { get; set; }
        public string? Metadata { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<RequestProperty> RequestProperties { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<StepConnectionsSchema> StepConnectionsSchemas { get; set; }
        public virtual ICollection<StepsSchema> StepsSchemas { get; set; }
    }
}
