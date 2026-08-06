using Employee.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.Repositories
{
    public interface IEmployeeRepository
    {
        Task<bool> ImportEmployeesFromFileAsync(List<Employee> employee);
        Task<bool> AddAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Employee employee);
        Task<PagedResult<Employee>> GetAllAsync(int page, int pageSize, string? searchTerm, DateTime? fromDate, DateTime? toDate, string? sortColumn, string? sortDirection);
        Task<Employee?> GetByIdAsync(int id);
    }
}
