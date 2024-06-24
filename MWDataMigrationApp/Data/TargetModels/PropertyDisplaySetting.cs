using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PropertyDisplaySetting
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public int LevelId { get; set; }
        public string? DisplayAreas { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
