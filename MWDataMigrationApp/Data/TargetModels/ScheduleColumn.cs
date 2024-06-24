using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ScheduleColumn
    {
        public ScheduleColumn()
        {
            ScheduleViewColumns = new HashSet<ScheduleViewColumn>();
        }

        public int Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<ScheduleViewColumn> ScheduleViewColumns { get; set; }
    }
}
