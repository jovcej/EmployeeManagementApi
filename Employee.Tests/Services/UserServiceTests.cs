using AutoMapper;
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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExist()
        {
            //Arrange
            var user = new User(
                new Username("John"),
                UserRole.user
            );

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            //Act
            var result = await _userService.GetUserById(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Username);
            Assert.Equal("user", result.Role);
        }

        [Fact]
        public async Task GetUserById_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            //Aeeange
            _userRepositoryMock 
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(
                async () => await _userService.GetUserById(1));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateRole_WhenUserExists()
        {
            //Arrange
            var user = new User(
                new Username("John"),
                UserRole.user);

            user.ChangePassword(
                new PasswordHash("hashed_password"));

            var request = new UpdateRequestDto
            {
                Role = "admin"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            //Act
            var result = await _userService.UpdateUserAsync(1, request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(UserRole.admin, user.Role);

            _userRepositoryMock.Verify(
                x => x.UpdateAsync(user),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            var request = new UpdateRequestDto
            {
                Role = "admin" 
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _userService.UpdateUserAsync(1, request));

            //Verify UpdateAsync was never called
            _userRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<User>()),
                Times.Never);
        }
    }
}
