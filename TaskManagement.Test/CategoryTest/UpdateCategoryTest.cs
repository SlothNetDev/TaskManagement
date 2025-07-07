using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Services.Categories.Command;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Entities;
using Xunit.Abstractions;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using MockQueryable;
using MockQueryable.Moq;

namespace TaskManagement.Test.CategoryTest
{
    public class UpdateCategoryServiceTest
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ILogger<UpdateCategoryService>> _mockLogger = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        private readonly IUpdateCategoryService _service;
    
        private readonly Guid _jwtUserId = Guid.NewGuid();
        private readonly Guid _domainUserId = Guid.NewGuid();
    
        private readonly ApplicationUsers _applicationUser;
        private readonly Category _existingCategory;
    
        public UpdateCategoryServiceTest(ITestOutputHelper output)
        {
            _output = output;
    
            // Set up application user and category
            _applicationUser = new ApplicationUsers { Id = _jwtUserId, DomainUserId = _domainUserId };
            _existingCategory = new Category
            {
                Id = Guid.NewGuid(),
                UserId = _domainUserId,
                CategoryName = "OldName",
                Description = "OldDescription"
            };
    
            // Set up JWT HttpContext
            HttpContextAccessorHelperTest.SetUpHttpContextAccessor(_jwtUserId, _mockHttpContextAccessor);
    
            // Set up DbContextMock
            var dbContext = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockDbContext = dbContext;
            dbContext.CreateDbSetMock(x => x.UserApplicationDb, new[] { _applicationUser });
            dbContext.CreateDbSetMock(x => x.CategoryDb, new[] { _existingCategory });
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    
            // Initialize service
            _service = new UpdateCategoryService(
                _mockDbContext.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object
            );
        }
    
        [Fact]
        public async Task UpdateCategories_Should_UpdateSuccessfully()
        {
            // Arrange
            var request = new CategoryUpdateDto
            {
                Id = _existingCategory.Id,
                CategoryName = "shuwa",
                Description = "Mona"
            };
            var assert = new AsserApiHelpers(_output);
    
            // Act
            var result = await _service.UpdateCategoriesAsync(request);
    
            // Assert
            assert.ShouldSucceed(result, "Category Updated Successfully");
            assert.ShouldMatch(result, data =>
                data.CategoryName == request.CategoryName &&
                data.Description == request.Description);
    
            // Verify interactions
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategories_Should_ReturnFail_WhenValidationFails()
        {
            // Arrange
            var assert = new AsserApiHelpers(_output);
            var invalidRequest = new CategoryUpdateDto
            {
                Id = Guid.Empty,
                CategoryName = new string('a', 5001),
                Description = new string('b', 1001)
            };
               
            // Act

            var result = await _service.UpdateCategoriesAsync(invalidRequest);
    
            // Assert
            assert.ShouldFail(result, "Field Request for Models has an Error");
        }
    
        [Fact]
        public async Task UpdateCategories_Should_ReturnFail_WhenCategoryNotFound()
        {
            // Arrange
            var assert = new AsserApiHelpers(_output);
            var missingId = Guid.NewGuid(); // Not in Db
            // Arrange
            var request = new CategoryUpdateDto
            {
                Id = missingId,
                CategoryName = "shuwa",
                Description = "Mona"
            };
    
            // Replace with empty CategoryDb
            _mockDbContext.Object.CategoryDb = new List<Category>().AsQueryable().BuildMockDbSet().Object;
    
            // Act
            var result = await _service.UpdateCategoriesAsync(request);
    
            // Assert
            assert.ShouldFail(result, "Category not found");
        }
    
        [Fact]
        public async Task UpdateCategories_Should_ReturnFailed_WhenDomainIdIsEmpty()
        {
            // Arrange
            var assert = new AsserApiHelpers(_output);

             // Create a user with an empty DomainUserId
            var userWithEmptyDomainId = new ApplicationUsers { Id = _jwtUserId, DomainUserId = Guid.Empty };
            var users = new List<ApplicationUsers> { userWithEmptyDomainId }.AsQueryable().BuildMockDbSet();
            _mockDbContext.Object.UserApplicationDb = users.Object;

           // The category list can be empty or as needed
            var categories = new List<Category>().AsQueryable().BuildMockDbSet();
            _mockDbContext.Object.CategoryDb = categories.Object;

    
            // Arrange
            var request = new CategoryUpdateDto
            {
                Id = userWithEmptyDomainId.DomainUserId,
                CategoryName = "shuwa",
                Description = "Mona"
            };
    
            // Act
            var result = await _service.UpdateCategoriesAsync(request);
    
            // Assert
            assert.ShouldFail(result, "Field Request for Models has an Error");
        }
    
        [Fact]
        public async Task UpdateCategories_Should_ReturnFail_WhenExceptionThrown()
        {
            // Arrange
            var assert = new AsserApiHelpers(_output);
            // Arrange
            var request = new CategoryUpdateDto
            {
                Id = _existingCategory.Id,
                CategoryName = "shuwa",
                Description = "Mona"
            };
    
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Something went wrong"));
    
            // Act
            var result = await _service.UpdateCategoriesAsync(request);
    
            // Assert
            assert.ShouldFail(result, "Failed to Update Categories");
        }
    }

        

    
    
}
