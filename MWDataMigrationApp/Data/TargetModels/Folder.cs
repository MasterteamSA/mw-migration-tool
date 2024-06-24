using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Folder
    {
        public Folder()
        {
            Files = new HashSet<File>();
        }

        public int Id { get; set; }
        public string? Entity { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int LevelDataId { get; set; }
        public int? LogDataId { get; set; }
        public bool IsPrimaryFolder { get; set; }
        public bool IsEditableFolder { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}
