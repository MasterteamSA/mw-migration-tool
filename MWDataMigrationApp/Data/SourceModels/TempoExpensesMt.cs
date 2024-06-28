using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class TempoExpensesMt
    {
        public string ExpenseId { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public long? ProjectId { get; set; }
        public double? AmountValue { get; set; }
        public DateTime? Date { get; set; }
    }
}
