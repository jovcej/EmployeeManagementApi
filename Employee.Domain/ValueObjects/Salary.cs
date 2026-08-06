using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.ValueObjects
{
    public class Salary
    {
        public decimal Amount { get; private set; }

        private Salary() 
        { 
        }

        public Salary(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Salary cannot be negative");

            Amount = amount;
        }
    }
}
