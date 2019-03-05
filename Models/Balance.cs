using System;

namespace ApolloWMS.Models
{
    public partial class Balance
    {
        public Guid BalanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal UsedBalance { get; set; }
        public decimal RemainedBalance { get; set; }
        public bool IsEdited { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
