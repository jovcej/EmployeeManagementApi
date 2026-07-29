using AutoMapper;
using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Core.Exceptions;
using Employee.Domain;
using Employee.Domain.Common;
using Employee.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IUserRepository = Employee.Domain.Repositories.IUserRepository;

namespace Employee.Core.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
      
        public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int page, int pageSize, string? searchTerm, string? sortColumn, string? sortDirection)
        {
            try
            {
                var result = await _userRepository.GetAllAsync(page, pageSize, searchTerm, sortColumn, sortDirection);

                var dtoList = _mapper.Map<List<UserResponseDto>>(result.Data);

                return new PagedResult<UserResponseDto>
                {
                    Data = dtoList,
                    TotalCount = result.TotalCount,
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured while getting users");
                throw;  
            }
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if(user == null)
                {
                    throw new NotFoundException("User not found");
                }

                return new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username.Value,
                    Role = user.Role.ToString()
                };
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching User by id: {UserId}.", id);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new NotFoundException("User not Found");
                }

                if (Enum.TryParse<UserRole>(
                    request.Role,
                    true,
                    out var role))
                {
                    user.ChangeRole(role);
                }
                else 
                {
                    throw new ArgumentException("Invalid user role");
                }

                await _userRepository.UpdateAsync(user);

                return new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username.Value,
                    Role = user.Role.ToString(),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured while updating users");
                throw;
            }
        }
    }
}
