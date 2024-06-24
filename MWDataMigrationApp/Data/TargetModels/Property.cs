using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Property
    {
        public Property()
        {
            PropertiesValues = new HashSet<PropertiesValue>();
            PropertyStatuses = new HashSet<PropertyStatus>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string Key { get; set; } = null!;
        public string? NormalizedKey { get; set; }
        public string ViewType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public bool IsTranslatable { get; set; }
        public bool IsSystem { get; set; }
        public bool IsForInternalPurpose { get; set; }
        public string? DefaultValue { get; set; }
        public string? Formula { get; set; }
        public string? FormulaRaw { get; set; }
        public string? Script { get; set; }
        public string? CalculationMethod { get; set; }
        public string? Configuration { get; set; }
        public int Order { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsHierarchyProperty { get; set; }
        public string? DependsOn { get; set; }
        public int? ValueOnCreation { get; set; }
        public int? LevelId { get; set; }
        public int? RefId { get; set; }
        public int? LevelLogId { get; set; }
        public int? LogId { get; set; }
        public bool IncludeInSummary { get; set; }
        public bool IsDisplayOverView { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsHiddenInCreation { get; set; }
        public string? Validation { get; set; }
        public bool? IsHiddenInEdition { get; set; }
        public string? Description { get; set; }
        public int? SectionId { get; set; }

        public virtual Level? Level { get; set; }
        public virtual LevelsLog? LevelLog { get; set; }
        public virtual Log? Log { get; set; }
        public virtual Level? Ref { get; set; }
        public virtual PropertySection? Section { get; set; }
        public virtual ICollection<PropertiesValue> PropertiesValues { get; set; }
        public virtual ICollection<PropertyStatus> PropertyStatuses { get; set; }
    }
}
