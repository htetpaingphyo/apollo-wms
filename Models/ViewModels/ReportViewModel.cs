using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models.ViewModels
{
    public class ReportViewModel
    {
        [Key]
        public Guid ReportId { get; set; }
        public string Email { get; set; }
        public int ReportingLevel { get; set; }
        public string ReportTo { get; set; }
    }
}
