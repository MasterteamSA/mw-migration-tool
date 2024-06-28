using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWDataMigrationApp.Models
{

    public class PaymentPlanItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double TotalCost { get; set; }
        public DateTime DueDate { get; set; }
        public List<int> Deliverables { get; set; }
        public List<int> Boqs { get; set; }

        public PaymentPlanItemVM()
        {
            Deliverables = new List<int>();
            Boqs = new List<int>();
        }
    }
}
