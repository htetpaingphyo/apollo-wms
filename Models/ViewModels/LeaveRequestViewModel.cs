using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models.ViewModels
{
    public class LeaveRequestViewModel
    {
        [Key]
        public Guid LeaveRequestId { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal Total { get; set; }
        public string ReasonForAbsence { get; set; }
        public string EmergencyContact { get; set; }
        public string Requester { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
