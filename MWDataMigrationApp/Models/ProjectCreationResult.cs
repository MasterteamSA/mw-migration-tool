using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWDataMigrationApp.Models
{
    public class ProjectCreationResult
    {
        public int ProjectId { get; set; }
        public bool DefaultStartDateAdded { get; set; }
        public bool DefaultEndDateAdded { get; set; }
        public bool DefaultManagerAdded { get; set; }
        public bool DefaultProgramDirectorAdded { get; set; }
        public bool DefaultAccountManagerAdded { get; set; }
    }
}
