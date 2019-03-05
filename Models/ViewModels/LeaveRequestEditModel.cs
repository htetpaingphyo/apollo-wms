using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models.ViewModels
{
    public class LeaveRequestEditModel
    {
        [Key]
        public Guid LeaveRequestId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal Total { get; set; }
        public string ReasonForAbsence { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRS { get; set; }
        public Guid RequesterId { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; }
    }
}
