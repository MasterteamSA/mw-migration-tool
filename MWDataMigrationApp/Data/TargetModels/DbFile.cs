using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class DbFile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public byte[]? Content { get; set; }
        public string? ContentType { get; set; }
        public long Length { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? RefId { get; set; }
    }
}
