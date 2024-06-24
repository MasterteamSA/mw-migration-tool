using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class LevelsLog
    {
        public LevelsLog()
        {
            Properties = new HashSet<Property>();
            PropertySections = new HashSet<PropertySection>();
        }

        public int Id { get; set; }
        public int LevelId { get; set; }
        public int LogId { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual Log Log { get; set; } = null!;
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<PropertySection> PropertySections { get; set; }
    }
}
