using Employee.Domain;
using Employee.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> AddAsync(User user);
        Task<PagedResult<User>> GetAllAsync(int page, int pageSize, string? searchTerm, string? sortColumn, string? sortDirection);
        Task<User?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(User user);
    }
}
