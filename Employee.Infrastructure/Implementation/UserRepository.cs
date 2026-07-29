using Employee.Core.DTO;
using Employee.Domain;
using Employee.Domain.Common;
using Employee.Domain.Enums;
using Employee.Domain.Repositories;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Infrastructure.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly EmployeeDbContext _context;

        public UserRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<PagedResult<User>> GetAllAsync(
            int page,
            int pageSize,
            string? searchTerm,
            string? sortColumn,
            string? sortDirection)
        {
            var query = _context.Users.AsQueryable();

            //Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if(Enum.TryParse<UserRole>(
                    searchTerm,
                    true,
                    out var role))
                {
                    query = query.Where(u =>
                        u.Username.Value.Contains(searchTerm) ||
                        u.Role == role);
                }
                else
                {
                    query = query.Where(u =>
                        u.Username.Value.Contains(searchTerm));
                }
            }

            //Total records before pagination
            var totalCount = await query.CountAsync();

            switch (sortColumn?.ToLower())
            {
                case "username":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Username.Value)
                        : query.OrderBy(x => x.Username.Value);
                    break;

                case "role":

                    query = sortDirection == "desc"
                        ? query.OrderByDescending(x => x.Role)
                        : query.OrderBy(x => x.Role);

                    break;

                default:

                    query = query.OrderBy(x => x.Username.Value);

                    break;
            }

            //Pagination
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Data = data,
                TotalCount = totalCount,
            };
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Username.Value == username);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
