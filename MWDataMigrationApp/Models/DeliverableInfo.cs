namespace MWDataMigrationApp.Models
{
    public class DeliverableInfo
    {
        public long MtDeliverablId {  get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PlannedFinishDate { get; set; }
        public double? EarnedValue {  get; set; }
        public string PaymentPlanStatus { get; set; }
        public double? Amount { get; set; }
        public string InvoiceNumber { get; set; }
        public int BoqId { get; set; }
        public int PaymentPlanItemId { get; set; }
        public int InvoiceId { get; set; }
    }
}
