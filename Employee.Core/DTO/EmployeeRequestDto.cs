using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.DTO
{
    public class EmployeeRequestDto
    {
        public required string Name { get; set; }

        public required string Surname { get; set; }
        public DateTime Date_Employee { get; set; }

        public decimal Salary { get; set; }


    }
}
