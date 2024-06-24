using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class ProjectEarnedValue
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public double ActualProgress { get; set; }
        public double Budget { get; set; }
        public double Spent { get; set; }
        public double EarnedValue { get; set; }
        public DateTime Cycle { get; set; }
    }
}
