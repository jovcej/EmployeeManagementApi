using Employee.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain
{
    public class Employee
    {
        public int Id { get; private set; }
        public EmployeeName Name { get; private set; } = null!;
        public EmployeeName Surname { get; private set; } = null!;
        public DateTime Date_employee { get; private set; }
        public Salary Salary { get; private set; } = null!;

        private Employee()
        {

        }

        public Employee(
            string name,
            string surname,
            DateTime date_employee,
            decimal salary_amount)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name required");

            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("Surname required");

            if (salary_amount < 0)
                throw new ArgumentException("Invalid salary");

            Name = new EmployeeName(name);
            Surname = new EmployeeName(surname);
            Date_employee = date_employee;
            Salary = new Salary(salary_amount);
        }

        public void ChangeSalaryAmount(decimal salary_amount)
        {
            if (salary_amount < 0)
                throw new ArgumentException("Invalid salary");

            Salary = new Salary(salary_amount);
        }

        public void UpdateInformation(
            string name,
            string surname,
            DateTime date_employee)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name required");

            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("Surname required");

            Name = new EmployeeName(name);
            Surname = new EmployeeName(surname);
            Date_employee = date_employee;
        }
    }
}
