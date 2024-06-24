using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class TimeSheetItem
    {
        public int Id { get; set; }
        public int TimeSheetId { get; set; }
        public int DeliverableId { get; set; }
        public int Hours { get; set; }

        public virtual Deliverable Deliverable { get; set; } = null!;
        public virtual TimeSheet TimeSheet { get; set; } = null!;
    }
}
