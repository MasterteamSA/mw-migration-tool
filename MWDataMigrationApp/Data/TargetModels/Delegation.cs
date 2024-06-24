using System;
using System.Collections.Generic;

namespace MWDataMigrationApp.Data.TargetModels
{
    public partial class Delegation
    {
        public int Id { get; set; }
        public string? DelegateFrom { get; set; }
        public string? DelegateTo { get; set; }
        public DateTime DelegateFromDateTime { get; set; }
        public DateTime DelegateToDateTime { get; set; }
        public string? DelegationReason { get; set; }
        public bool DelegationStatus { get; set; }
        public string? DelegateRequestPayload { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
