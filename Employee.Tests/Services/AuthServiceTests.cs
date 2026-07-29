using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Core.Exceptions;
using Employee.Core.Implementation;
using Employee.Domain;
using Employee.Domain.Enums;
using Employee.Domain.Repositories;
using Employee.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMOck;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _passwordHasherMOck = new Mock<IPasswordHasher<User>>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                _passwordHasherMOck.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_whenUsernameDoesNotExist()
        {
            //Arrange
            var request = new RegisterRequestDto
            {
                Username = "John",
                Password = "Password123"
            };

            //No existing user
            _userRepositoryMock
                .Setup(x => x.GetByUsernameAsync(request.Username))
                .ReturnsAsync((User)null);

            //Mock password hashing
            _passwordHasherMOck
                .Setup(x => x.HashPassword(
                    It.IsAny<User>(),
                    request.Password))
                .Returns("hashed_password");

            //Act
            var result = await _authService.RegisterAsync(request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Username);
            Assert.Equal("user", result.Role);

            //Verify that user was saved
            _userRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<User>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowConflictException_WhenUsernameAlreadyExist()
        {
            //Arrange
            var request = new RegisterRequestDto
            {
                Username = "John",
                Password = "Password123"
            };

            var existingUser = new User(
                new Domain.ValueObjects.Username("John"),
                Domain.Enums.UserRole.user
            );

            _userRepositoryMock
                .Setup(x => x.GetByUsernameAsync(request.Username))
                .ReturnsAsync(existingUser);

            //Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                async () =>
                {
                    await _authService.RegisterAsync(request);
                });

            //Verify user was not saved
            _userRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            //Arrange
            var username = "John";
            var password = "password123";

            var user = new User(
                new Domain.ValueObjects.Username(username),
                UserRole.user
            );

            user.ChangePassword(
                 new PasswordHash("hashed_password"));

            _userRepositoryMock
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    password))
                .Returns(PasswordVerificationResult.Success);

            _jwtServiceMock
                .Setup(x => x.GenerateToken(user))
                .Returns("jwt_token");

            //Act
            var result = await _authService.LoginAsync(username, password);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("jwt_token", result);
            
            _jwtServiceMock.Verify(
                x => x.GenerateToken(user),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUsernameDoesNotExist()
        {
            //Arrange
            var username = "unknown";
            var password = "Password123";

            _userRepositoryMock
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                async () =>
                {
                    await _authService.LoginAsync(username, password);
                });

            //Verify password is never checked
            _passwordHasherMOck.Verify(
                x => x.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            //Verify token is never generated

            _jwtServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsWrong()
        {
            //Arrange
            var username = "John";
            var password = "WromgPassword";

            var user = new User(
                new Username(username),
                UserRole.user
            );

            user.ChangePassword(
                new PasswordHash("hashed_password")
            );

            _userRepositoryMock
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync((user));

            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    password))
                .Returns(PasswordVerificationResult.Failed);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () =>
                {
                    await _authService.LoginAsync(
                        username,
                        password);
                });

            //Verify Jwt token was never generated
            _jwtServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldChangePassworf_WhenUserExists()
        {
            //Arrange
            var user = new User(
                new Username("John"),
                UserRole.user);

            user.ChangePassword(
                new PasswordHash("old_hash"));

            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.CurrentPassword))
                .Returns(PasswordVerificationResult.Success);

            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.NewPassword))
                .Returns(PasswordVerificationResult.Failed);

            _passwordHasherMOck
                .Setup(x => x.HashPassword(
                    user,
                    request.NewPassword))
                .Returns("new_hash");

            //Act
            var result = await _authService.ChangePasswordAsync(request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("new_hash", user.PasswordHash.Value);

            _passwordHasherMOck.Verify(
                x => x.HashPassword(
                    user,
                    request.NewPassword),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.UpdateAsync(user),
                Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizeException_WhenCurrentPasswordIsWrong()
        {
            //Arrange
            var user = new User(
                new Username("John"),
                UserRole.user);

            user.ChangePassword(
                new PasswordHash("old_hash"));

            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "WrongPassword",
                NewPassword = "NewPassword123"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            //Current password is wrong
            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.CurrentPassword))
                .Returns(PasswordVerificationResult.Failed);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                async () =>
                {
                    await _authService.ChangePasswordAsync(request);
                });

            //Verify that password is not changed
            _userRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowConflictException_WhenNewPasswordIsSameAsCurrentPassword()
        {
            //Arrange
            var user = new User(
                new Username("John"),
                UserRole.user);

            user.ChangePassword(
                new PasswordHash("old_hash"));

            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "OldPassword123"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            //Currrent password is correct
            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.CurrentPassword))
                .Returns(PasswordVerificationResult.Success);

            _passwordHasherMOck
                .Setup(x => x.VerifyHashedPassword(
                    user,
                    user.PasswordHash.Value,
                    request.NewPassword))
                .Returns(PasswordVerificationResult.Success);

            //Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                async () =>
                {
                    await _authService.ChangePasswordAsync(request);
                });

            //Password should not be change
            _passwordHasherMOck.Verify(
                x => x.HashPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>()),
                Times.Never);

            //Database should not be updated
            _userRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<User>()),
                Times.Never);
        }
    }
}
