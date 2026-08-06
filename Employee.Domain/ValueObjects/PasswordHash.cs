using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.ValueObjects
{
    public class PasswordHash
    {
        public string Value { get; private set; } = string.Empty;
        private PasswordHash() { }

        public PasswordHash(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password hash cannot be empty");

            Value = value;
        }
    }
}
