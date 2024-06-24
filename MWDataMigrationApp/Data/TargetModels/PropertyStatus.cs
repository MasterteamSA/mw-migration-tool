using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class PropertyStatus
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string Type { get; set; } = null!;
        public string? Display { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public string? Formula { get; set; }
        public string? FormulaRaw { get; set; }
        public int PropertyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Property Property { get; set; } = null!;
    }
}
