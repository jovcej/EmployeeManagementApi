using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.ValueObjects
{
    public class Username
    {
        public string Value { get; private set; } = string.Empty;

        private Username() { }
        public Username(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Username cannot be empty");

            if (value.Length < 4)
                throw new ArgumentException("Username must have at least 3 characters");

            Value = value;
        }
    }
}
