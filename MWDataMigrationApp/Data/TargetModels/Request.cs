using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Request
    {
        public Request()
        {
            Histories = new HashSet<History>();
            RequestPropertyValues = new HashSet<RequestPropertyValue>();
            Steps = new HashSet<Step>();
        }

        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public string? Metadata { get; set; }
        public bool IsActive { get; set; }
        public int RequestSchemaId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public virtual RequestsSchema RequestSchema { get; set; } = null!;
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<RequestPropertyValue> RequestPropertyValues { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
    }
}
