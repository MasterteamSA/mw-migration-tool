using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class MatrixValue
    {
        public int Id { get; set; }
        public string? Log { get; set; }
        public string? PropertyName { get; set; }
        public string? Value { get; set; }
        public string? Pattern { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
