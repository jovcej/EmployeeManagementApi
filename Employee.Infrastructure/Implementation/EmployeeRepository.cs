using Employee.Core.DTO;
using Employee.Domain;
using Employee.Domain.Common;
using Employee.Domain.Repositories;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Infrastructure.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _context;

        public EmployeeRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Domain.Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return false;
            }

            _context.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<Domain.Employee>> GetAllAsync(
            int page,
            int pageSize,
            string? searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortColumn,
            string? sortDirection)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                if (decimal.TryParse(searchTerm, out decimal salary))
                {
                    query = query.Where(x =>
                        x.Name.Value.Contains(searchTerm) ||
                        x.Surname.Value.Contains(searchTerm) ||
                        x.Salary.Amount == salary);
                }
                else
                {
                    query = query.Where(x =>
                    x.Name.Value.Contains(searchTerm) ||
                    x.Surname.Value.Contains(searchTerm));
                }
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date_employee >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x =>
                    x.Date_employee <= toDate.Value.Date.AddDays(1));
            }

            var totalCount = await query.CountAsync();

            switch(sortColumn?.ToLower())
            {
                case "name":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Name.Value)
                        : query.OrderBy(x => x.Name.Value); 
                    break;

                case "surname":

                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Surname.Value)
                        : query.OrderBy(x => x.Surname.Value);

                    break;


                case "date_employee":

                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Date_employee)
                        : query.OrderBy(x => x.Date_employee);

                    break;


                case "salary":

                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Salary.Amount)
                        : query.OrderBy(x => x.Salary.Amount);

                    break;


                default:

                    query = query.OrderBy(x => x.Name.Value);

                    break;
            }

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Domain.Employee>
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        public async Task<Domain.Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<bool>ImportEmployeesFromFileAsync(List<Domain.Employee> employee)
        {
            await _context.AddRangeAsync(employee);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> UpdateAsync(Domain.Employee employee)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
