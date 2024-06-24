using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class File
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Body { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public string? RefId { get; set; }
        public int? FolderId { get; set; }

        public virtual Folder? Folder { get; set; }
    }
}
