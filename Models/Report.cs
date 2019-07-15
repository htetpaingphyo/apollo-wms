using System;

namespace ApolloWMS.Models
{
    public partial class Report
    {
        public Guid ReportId { get; set; }
        public Guid EmployeeId { get; set; }
        public int ReportingLevel { get; set; }
        public Guid? ReportTo { get; set; }
    }
}
