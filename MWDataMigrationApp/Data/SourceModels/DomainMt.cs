using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class DomainMt
    {
        public int SeqId { get; set; }
        public string DomainName { get; set; }
        public string UserAccountId { get; set; }
        public string ProgramDirector { get; set; }
    }
}
