using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Template
    {
        public Template()
        {
            Receivers = new HashSet<Receiver>();
        }

        public int Id { get; set; }
        public string? EmailContent { get; set; }
        public string? Smscontent { get; set; }
        public string? PushContent { get; set; }
        public bool IsSystem { get; set; }
        public int EventId { get; set; }
        public string? Name { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual ICollection<Receiver> Receivers { get; set; }
    }
}
