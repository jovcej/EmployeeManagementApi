using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.DTO
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public decimal Salary { get; set; }
        public DateTime Date_employee { get; set; }
    }
}
