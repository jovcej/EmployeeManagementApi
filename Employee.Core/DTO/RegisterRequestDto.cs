using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.DTO
{
    public class RegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; } 
    }
}
