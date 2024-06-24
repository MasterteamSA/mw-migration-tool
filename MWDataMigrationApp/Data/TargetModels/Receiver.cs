using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Receiver
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string ReceiverType { get; set; } = null!;
        public bool IsSystem { get; set; }
        public int TemplateId { get; set; }

        public virtual Template Template { get; set; } = null!;
    }
}
