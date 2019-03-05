using System;
using System.ComponentModel.DataAnnotations;

namespace ApolloWMS.Models
{
    public partial class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Designation { get; set; }
        public string Region { get; set; }
        public Guid EmployeeTypeId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? HiredDate { get; set; }
        public string ContactNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
