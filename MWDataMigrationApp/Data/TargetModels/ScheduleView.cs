using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ScheduleView
    {
        public ScheduleView()
        {
            ScheduleViewColumns = new HashSet<ScheduleViewColumn>();
        }

        public int Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<ScheduleViewColumn> ScheduleViewColumns { get; set; }
    }
}
