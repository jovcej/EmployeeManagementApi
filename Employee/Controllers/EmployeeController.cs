using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Domain;
using Employee.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Employee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService) // Add constructor to initialize _configuration
        {
            _employeeService = employeeService;
        }

        [HttpPost("ImportEmployee")]
        public async Task<IActionResult> ImportEmployeeAsync(IFormFile file)
        {
            var result = await _employeeService.ImportEmployeesFromFileAsync(file.OpenReadStream());

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeRequestDto request)
        {
            var result = await _employeeService.AddEmployeeAsync(request);

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            return Ok(result);

        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(int id, EmployeeRequestDto employee)
        {
            var result = await _employeeService.UpdateEmployeeAsync(id, employee);

            return Ok(result);
        }

        [HttpGet("GetEmployee")]
        public async Task<ActionResult<PagedResult<EmployeeResponseDto>>> GetEmployee(
            int page = 1,
            int pageSize = 5,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortColumn = "name",
            string? sortDirection = "asc")
        {
            var result = await _employeeService.GetAllEmployeesAsync(page, pageSize, searchTerm, fromDate, toDate, sortColumn, sortDirection);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(result);
        }
    }
}
