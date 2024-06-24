using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Event
    {
        public Event()
        {
            Templates = new HashSet<Template>();
        }

        public int Id { get; set; }
        public string EventName { get; set; } = null!;
        public int ModuleId { get; set; }
        public string DisplayName { get; set; } = null!;
        public string ModuleType { get; set; } = null!;

        public virtual ICollection<Template> Templates { get; set; }
    }
}
