using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PropertySection
    {
        public PropertySection()
        {
            Properties = new HashSet<Property>();
        }

        public int Id { get; set; }
        public string? Key { get; set; }
        public int? LevelId { get; set; }
        public int? LevelLogId { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int Order { get; set; }

        public virtual Level? Level { get; set; }
        public virtual LevelsLog? LevelLog { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
