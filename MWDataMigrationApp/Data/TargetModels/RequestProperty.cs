using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class RequestProperty
    {
        public RequestProperty()
        {
            PropertiesSteps = new HashSet<PropertiesStep>();
            RequestPropertyValues = new HashSet<RequestPropertyValue>();
        }

        public int Id { get; set; }
        public int RefId { get; set; }
        public string? Type { get; set; }
        public string? Metadata { get; set; }
        public int RequestSchemaId { get; set; }
        public bool IsDeleted { get; set; }
        public string? RefType { get; set; }

        public virtual RequestsSchema RequestSchema { get; set; } = null!;
        public virtual ICollection<PropertiesStep> PropertiesSteps { get; set; }
        public virtual ICollection<RequestPropertyValue> RequestPropertyValues { get; set; }
    }
}
