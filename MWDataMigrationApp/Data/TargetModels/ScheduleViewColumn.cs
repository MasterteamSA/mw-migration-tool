using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ScheduleViewColumn
    {
        public int ScheduleViewId { get; set; }
        public int ScheduleColumnId { get; set; }
        public bool IsHidden { get; set; }
        public int Order { get; set; }

        public virtual ScheduleColumn ScheduleColumn { get; set; } = null!;
        public virtual ScheduleView ScheduleView { get; set; } = null!;
    }
}
