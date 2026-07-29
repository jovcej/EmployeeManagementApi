using Employee.Core.DTO;
using Employee.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Abstraction
{
    public interface IEmployeeService
    {
        Task<ImportResponseDto> ImportEmployeesFromFileAsync(Stream fileStream);
        Task<ApiRespomseDto> AddEmployeeAsync(EmployeeRequestDto request);
        Task<ApiRespomseDto> DeleteEmployeeAsync(int id);
        Task<ApiRespomseDto> UpdateEmployeeAsync(int id, EmployeeRequestDto employee);
        Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(int page, int pageSize, string? searchTerm, DateTime? fromDate, DateTime? toDate, string? sortColumn, string? sortDirection);
        Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id);
    }
}
