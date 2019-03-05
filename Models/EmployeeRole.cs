using System;
using System.Collections.Generic;

namespace ApolloWMS.Models
{
    public partial class EmployeeRole
    {
        public Guid EmployeeRoleId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid RoleId { get; set; }
    }
}
