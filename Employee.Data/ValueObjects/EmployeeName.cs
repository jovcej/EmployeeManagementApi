using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.ValueObjects
{
    public class EmployeeName
    {
        public string Value { get; private set; } = string.Empty;

        private EmployeeName() 
        {
        }

        public EmployeeName(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Employee name is required");

            Value = value;
        }
    }
}
