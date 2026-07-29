using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Employee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetUsers(
            int page = 1,
            int pageSize = 5,
            string? searchTerm = null,
            string? sortColumn = "name",
            string? sortDirection = "asc")
        {

            var result = await _userService.GetAllUsersAsync(page, pageSize, searchTerm, sortColumn, sortDirection);

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, UpdateRequestDto request)
        {
            var result = await _userService.UpdateUserAsync(id, request);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            return Ok(user);
        }
    }
}
