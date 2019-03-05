using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models.ViewModels
{
    public class BalanceViewModel
    {
        [Key]
        public Guid BalanceId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal UsedBalance { get; set; }
        public decimal RemainedBalance { get; set; }
        public bool IsEdited { get; set; }
    }
}
