using AutoMapper;
using CsvHelper;
using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Domain.Common;
using Employee.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Globalization;
using IEmployeeRepository = Employee.Domain.Repositories.IEmployeeRepository;

namespace Employee.Core.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public EmployeeService(IConfiguration configuration, IMapper mapper, ICacheService cacheService, IEmployeeRepository employeeRepository)
        {
            _configuration = configuration;
            _mapper = mapper;
            _cacheService = cacheService;
            _employeeRepository = employeeRepository;
        }

        public async Task<ApiRespomseDto> AddEmployeeAsync(EmployeeRequestDto request)
        {
            try
            {
                var employee = new Domain.Employee(
                    request.Name,
                    request.Surname,
                    request.Date_Employee,
                    request.Salary
                );

                await _employeeRepository.AddAsync(employee);

                return new ApiRespomseDto
                {
                    Message = "Employee added succesfully"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while adding employee");
                throw;
            }
        }

        public async Task<ApiRespomseDto> DeleteEmployeeAsync(int id)
        {
            try 
            {
                var result = await _employeeRepository.DeleteAsync(id);
                if (!result)
                {
                    Log.Warning("Employee with id {EmployeeId} not found for deletion.", id);

                    return new ApiRespomseDto
                    {
                        Message = "Employee not found"
                    };
                }

                // REMOVE OLD CACHE
                //_cacheService.RemoveByPrefix("employees");

                return new ApiRespomseDto
                {
                    Message = "Employee deleted successfully"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting employee with id {EmployeeId}.", id);
                throw;
            }
        }

        public async Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(
            int page,
            int pageSize,
            string? searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortColumn,
            string? sortDirection)
        {
            try
            {
                //string key = $"employees_{page}_{pageSize}";

                //var cached = _cacheService.Get<PagedResult<EmployeeResponseDto>>(key);

                //if (cached != null)
                //{ 
                //    return cached;
                //}

                var result = await _employeeRepository.GetAllAsync(page, pageSize, searchTerm, fromDate, toDate,sortColumn, sortDirection);

                var dtoList = _mapper.Map<List<EmployeeResponseDto>>(result.Data);

                var response = new PagedResult<EmployeeResponseDto>
                {
                    Data = dtoList,
                    TotalCount = result.TotalCount
                };

                //_cacheService.Set(key, response, TimeSpan.FromMinutes(5));

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error ocurred while fetching data");
                throw;
            }
        }

        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                var dtoList = _mapper.Map<EmployeeResponseDto>(employee);

                return dtoList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching employee by id: {EmployeeId}.", id);
                throw;
            }
        }

        public async Task<ImportResponseDto> ImportEmployeesFromFileAsync(Stream fileStream)
        {
            try
            {
                using var reader = new StreamReader(fileStream);

                using var csv = new CsvReader(
                    reader,
                    CultureInfo.InvariantCulture);

                var employeeDtos = csv.GetRecords<ImportRequestDto>().ToList();


                var emp = employeeDtos.Select(record =>
                    new Domain.Employee
                    (
                        record.Name,
                        record.Surname,
                        DateTime.ParseExact(
                            record.Date_Employee,
                            "yyyyMMdd",
                            CultureInfo.InvariantCulture),
                        record.Salary
                    )).ToList();

                await _employeeRepository.ImportEmployeesFromFileAsync(emp);

                // REMOVE OLD CACHE
                //_cacheService.RemoveByPrefix("employees");

                return new ImportResponseDto
                {
                    Message = "File uploaded succesfully",
                    ImportedCount = emp.Count
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while importing employees from file.");
                throw;
            }
        }

        public async Task<ApiRespomseDto> UpdateEmployeeAsync(int id, EmployeeRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Log.Warning("Attempted to update employee with id {EmployeeId} but provided employee data is null.", id);

                    return new ApiRespomseDto
                    {
                        Message = "Employee data is required"
                    };
                }

                var employeeExists = await _employeeRepository.GetByIdAsync(id);
                if (employeeExists == null)
                {
                    Log.Warning("Employee with id {EmployeeId} not found for update.", id);

                    return new ApiRespomseDto
                    {
                        Message = "Employee not found"
                    };
                }

                employeeExists.UpdateInformation(
                    dto.Name,
                    dto.Surname,
                    dto.Date_Employee
                );

                employeeExists.ChangeSalaryAmount( 
                    dto.Salary
                );


                var result = await _employeeRepository.UpdateAsync(employeeExists);

                // REMOVE OLD CACHE
                //_cacheService.RemoveByPrefix("employees");

                return new ApiRespomseDto
                {
                    Message = "Employee updated successfully"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating employee with id {EmployeeId}.", id);
                throw;
            }
        }
    }
}
