using Castle.Core.Logging;
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Enums;
using Xunit.Abstractions;
// Add this alias:

namespace TaskManagement.Test.ServiceTest
{
    public class CreateTaskServiceTest
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ILogger<CreateTaskService>> _mockLogger = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
    
        private readonly CreateTaskService _service;
    
        private readonly Guid _jwtUserId = Guid.NewGuid();
        private readonly Guid _domainUserId = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();
    
        private readonly TaskRequestDto _validRequest;
        private readonly ApplicationUsers _applicationUser;
        private readonly Category _category;
    
        public CreateTaskServiceTest(ITestOutputHelper output)
        {
            _output = output;
            // Arrange: test entities
            _applicationUser = new ApplicationUsers
            {
                Id = _jwtUserId,
                DomainUserId = _domainUserId
            };
            _category = new Category
            {
                Id = _categoryId,
                UserId = _domainUserId,
                CategoryName = "Test Category"
            };
    
            _validRequest = new TaskRequestDto
            {
                Title = "Test Task",
                CategoryId = _categoryId,
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = Priority.High
            };
    
            // Set up fake HttpContext with claims
            SetupHttpContextWithJwt(_jwtUserId);
    
            // Set up DbContextMock with stubbed data
            var dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockDbContext = dbContextMock;
            dbContextMock.CreateDbSetMock(x => x.UserApplicationDb, new[] { _applicationUser });
            dbContextMock.CreateDbSetMock(x => x.CategoryDb, new[] { _category });
            dbContextMock.CreateDbSetMock(x => x.TaskDb, new List<TaskItem>());
    
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    
            _service = new CreateTaskService(
                _mockDbContext.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object
            );
        }
    
        private void SetupHttpContextWithJwt(Guid userId)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
    
            var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")) };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }
        
        /// <summary>
        /// Check if create task was successfull
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateTaskAsync_ValidRequest_ReturnsSuccess()
        {
            AsserApiHelpers assert = new(_output);
            // Act
            var result = await _service.CreateTaskAsync(_validRequest);

            // Assert
            assert.ShouldSucceed(result, "Task Created Successfully");
            assert.ShouldMatch(result, data =>
                data.Title == _validRequest.Title &&
                data.Priority == _validRequest.Priority.ToString() &&
                data.DueDate == _validRequest.DueDate);

            _mockDbContext.Verify(db => db.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}
