using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models
{
    public partial class Holidays
    {
        public Guid HolidayId { get; set; }
        public string HolidayName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DefinedDate { get; set; }
        public int EncompassedYear { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
