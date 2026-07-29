using Azure.Core;
using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Core.Exceptions;
using Employee.Domain;
using Employee.Domain.Enums;
using Employee.Domain.Repositories;
using Employee.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly Domain.Repositories.IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(Domain.Repositories.IUserRepository userRepository, IJwtService jwtService, IPasswordHasher<User> passwordHasher, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);

                if (user == null)
                {
                    Log.Warning(
                        "Login failed. Username {Username} not found",
                        username
                    );

                    throw new UnauthorizedException("Invalid username or password");
                }

                var result = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    password
                );

                if (result == PasswordVerificationResult.Failed)
                {
                    Log.Warning(
                        "Login failed. Invalid password for username {Username}",
                        username
                    );

                    throw new UnauthorizedAccessException("Invalid username or password");
                }

                Log.Information(
                    "User {Username} logged in successfully",
                    username
                );

                return _jwtService.GenerateToken(user);
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            { 
                Log.Error(ex, "Login failed for username: {Username}");
                throw;
            }
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);

                if (existingUser != null)
                {
                    Log.Warning(
                       "Registration failed. Username {Username} already exists",
                        request.Username
                    );

                    throw new ConflictException("Username already exists");
                }

                var username = new Username(request.Username);

                var user = new User(
                    username,
                    UserRole.user
                );

                var passwordHash = _passwordHasher.HashPassword(user, request.Password);

                var passwordHashObj = new PasswordHash(passwordHash);
                user.ChangePassword(passwordHashObj);

                await _userRepository.AddAsync(user);

                Log.Information(
                    "User {Username} registered successfully",
                    request.Username
                );

                return new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username.Value,
                    Role = user.Role.ToString()
                };
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Registration failed for username: {Username}", request.Username);
                throw;
            }
        }

        public async Task<ApiRespomseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NewPassword) ||
                    request.NewPassword.Length < 4)
                {

                    Log.Warning(
                        "Password change failed. New password validation failed for user {UserId}",
                        _currentUserService.UserId);

                    throw new BadRequestException(
                        "Password must contain at least 4 characters.");
                }
                var userId = _currentUserService.UserId;

                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    Log.Warning(
                        "Password change failed. User {UserId} not found",
                        userId);

                    throw new NotFoundException("User not found");
                }

                var passwordResult = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.CurrentPassword);

                if (passwordResult == PasswordVerificationResult.Failed)
                {
                    Log.Warning(
                       "Password change failed. Invalid current password for user {UserId}",
                       userId);

                    throw new UnauthorizedException("Current password is incorrect.");
                }

                var samePassword = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.NewPassword);

                if (samePassword == PasswordVerificationResult.Success)
                {
                    Log.Warning(
                        "Password change failed. New password same as old password for user {UserId}",
                        userId);

                    throw new ConflictException(
                        "New password must be different from the current password.");
                }

                var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);

                var passwordHash = new PasswordHash(hashedPassword);

                user.ChangePassword(passwordHash);

                await _userRepository.UpdateAsync(user);

                Log.Information(
                  "Password changed successfully for user {UserId}",
                  userId);

                return new ApiRespomseDto
                {
                    Message = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured while updating password");
                throw;
            }
        }
    }
}
