using System;

namespace ApolloWMS.Models
{
    public partial class LeaveRequest
    {
        public Guid LeaveRequestId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public DateTime FromTimeOff { get; set; }
        public DateTime ToTimeOff { get; set; }
        public decimal TotalTimeOff { get; set; }
        public string ReasonForAbsence { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRS { get; set; }
        public Guid RequesterId { get; set; }
        public DateTime RequestedDate { get; set; }
        public Guid? AuthorizerId { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
