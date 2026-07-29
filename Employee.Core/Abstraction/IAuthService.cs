using Employee.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Abstraction
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<UserResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<ApiRespomseDto> ChangePasswordAsync(ChangePasswordRequestDto request);
    }
}
