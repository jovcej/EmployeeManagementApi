using AutoMapper;
using Employee.Core.Abstraction;
using Employee.Core.DTO;
using Employee.Core.Implementation;
using Employee.Domain.Repositories;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Employee.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ICacheService> _cacheServiceMock;

        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _cacheServiceMock = new Mock<ICacheService>();

            _employeeService = new EmployeeService(
                _configurationMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _employeeRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldAddEmployee_WhenRequestIsValid()
        {
            //Arrange
            var request = new EmployeeRequestDto
            {
                Name = "John",
                Surname = "Smith",
                Date_Employee = new DateTime(2026, 1, 1),
                Salary = 1500
            };

            _employeeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Employee.Domain.Employee>()))
                .ReturnsAsync(true);

            //Act
            var result = await _employeeService.AddEmployeeAsync(request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Employee added succesfully",
                result.Message);

            _employeeRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Employee.Domain.Employee>()),
                Times.Once);
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldThrowException_WhenRepositoryFails()
        {
            //Arrange
            var request = new EmployeeRequestDto
            {
                Name = "John",
                Surname = "Smith",
                Date_Employee = new DateTime(2016, 1, 1),
                Salary = 1500
            };

            _employeeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Employee.Domain.Employee>()))
                .ThrowsAsync(new Exception("Database error"));

            //Act & Assert
            await Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _employeeService.AddEmployeeAsync(request);
                });

            //Verify repository was called
            _employeeRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Employee.Domain.Employee>()),
                Times.Once);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenEmployeeExists()
        {
            //Arrange
            var employee = new Employee.Domain.Employee(
                "John",
                "Smith",
                new DateTime(2026, 1, 1),
                1500);

            var employeeDto = new EmployeeResponseDto
            {
                Name = "John",
                Surname = "Smith",
                Salary = 1500
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(employee);

            _mapperMock
                .Setup(x => x.Map<EmployeeResponseDto>(employee))
                .Returns(employeeDto);

            //Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            Assert.Equal("Smith", result.Surname);

            _employeeRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<EmployeeResponseDto>(employee),
                Times.Once);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {
            //Arrange
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Employee.Domain.Employee)null);

            _mapperMock
                .Setup(x => x.Map<EmployeeResponseDto>(null))
                .Returns((EmployeeResponseDto)null);

            //Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            //Assert
            Assert.Null(result);

            _employeeRepositoryMock .Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _mapperMock .Verify(
                x => x.Map<EmployeeResponseDto>(null),
                Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldUpdateEmployee_WhenEmployeeExists()
        {
            //Arrange
            var employee = new Employee.Domain.Employee(
                "oldName",
                "oldSurname",
                new DateTime(2026, 1, 1),
                1000);

            var request = new EmployeeRequestDto
            {
                Name = "John",
                Surname = "Smith",
                Date_Employee = new DateTime(2026, 1, 1),
                Salary = 2000
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(employee);

            _employeeRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Employee.Domain.Employee>()))
                .ReturnsAsync(true);

            //Act
            var result = await _employeeService.UpdateEmployeeAsync(1, request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Employee updated successfully",
                result.Message);

            _employeeRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _employeeRepositoryMock .Verify(
                x => x.UpdateAsync(employee),
                Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            //Arrange
            var request = new EmployeeRequestDto
            {
                Name = "John",
                Surname = "Smith",
                Date_Employee = new DateTime(2026, 1, 1),
                Salary = 2000
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Employee.Domain.Employee)null);

            //Act
            var result = await _employeeService.UpdateEmployeeAsync(1, request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(
               "Employee not found",
               result.Message);

            _employeeRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            //Update should never happen
            _employeeRepositoryMock .Verify(
                x => x.UpdateAsync(It.IsAny<Employee.Domain.Employee>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ShouldDeleteEmployee_WhenEmployeeExists()
        {
            //Arrange
            _employeeRepositoryMock
                .Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);

            //Act
            var result = await _employeeService.DeleteEmployeeAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Employee deleted successfully",
                result.Message);

            _employeeRepositoryMock.Verify(
                x => x.DeleteAsync(1),
                Times.Once);

        }

        [Fact]
        public async Task DeleteEmployeeAsync_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            //Arrange
            _employeeRepositoryMock
                .Setup (x => x.DeleteAsync(1)) 
                .ReturnsAsync(false);

            //Act
            var result = await _employeeService.DeleteEmployeeAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Employee not found",
                result.Message);

            _employeeRepositoryMock.Verify(
                x => x.DeleteAsync(1),
                Times.Once);
        }

    }
}
