using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Core.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _service.LoginAsync(dto.Username, dto.Password);

            return Ok(new{token});
        }

        [HttpPost("Register")]
        public async Task<IActionResult>Register(RegisterRequestDto requestDto)
        {
            var ret = await _service.RegisterAsync(requestDto);

            return Ok(ret);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChnagePassword(ChangePasswordRequestDto request)
        {
            var result = await _service.ChangePasswordAsync(request);

            return Ok(result);
        }
    }
}
