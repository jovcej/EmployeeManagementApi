using Employee.Core.DTO;
using Employee.Domain.Common;

namespace Employee.Core.Abstraction
{
    public interface IUserService
    {
        Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int page, int pageSize, string? searchTerm, string? sortColumn, string? sortDirection);
        Task<UserResponseDto> UpdateUserAsync(int id, UpdateRequestDto request);
        Task<UserResponseDto> GetUserById(int id);
    }
}
