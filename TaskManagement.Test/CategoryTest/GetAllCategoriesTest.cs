/*
using Castle.Core.Logging;
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Services.Categories.Query;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Domains.Entities;
using Xunit.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Test.CategoryTest
{
    public class GetAllCategoriesTest
    {
        private readonly ITestOutputHelper _output;
        private Mock<ApplicationDbContext> _mockDbContext;

        private Guid _jwtUserId = Guid.NewGuid();
        private readonly ApplicationUsers _applicationUser;
        public GetAllCategoriesTest(ITestOutputHelper output)
        {
            _output = output;
            //initialize test data
            _applicationUser = HelperTestMethod.CreateTestUser(_jwtUserId);
              

        }
        private Mock<ApplicationDbContext> GetMockDbContext(ApplicationUsers _applicationUser,List<Category> _category)
        {
             // Set up DbContextMock
           var dbContext = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockDbContext = dbContext;
            dbContext.CreateDbSetMock(x => x.UserApplicationDb, new[] { _applicationUser });
            dbContext.CreateDbSetMock(x => x.CategoryDb, _category ?? new List<Category>());
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            return dbContext;
        }
        private GetAllCategoriesService GetGetAllCategoriesUnderTest(List<Category> categories)
        {
             var _mockLogger = new Mock<ILogger<GetAllCategoriesService>>();
             var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            HelperTestMethod.SetUpHttpContextAccessor(_jwtUserId, _mockHttpContextAccessor);

            _mockDbContext = GetMockDbContext(_applicationUser, categories);

            return new GetAllCategoriesService(_mockDbContext.Object,
                _mockLogger.Object, _mockHttpContextAccessor.Object);
        }
        [Fact]
        public async Task GetAll_WithValidUser_Should_ReturnAllUserEntities()
        {
            // Arrange
            var testCategories = HelperTestMethod.CreateListTestCategories( _applicationUser,_output,1);
            var service = GetGetAllCategoriesUnderTest(testCategories);


            var result = await service.GetAllCategoriesAsync();

            var assert = new AssertApiHelpers(_output);
            assert.ShouldSucceed(result, "Successfully retrieved all categories.");
        }
        [Fact]
        public async Task GetAll_WithNoData_Should_ReturnEmptyList()
        {
            var service = GetGetAllCategoriesUnderTest(null);
            var result = await service.GetAllCategoriesAsync();

            var assert = new AssertApiHelpers(_output);
            assert.ShouldSucceed(result, "No categories found. Consider creating one.");
        }
        [Fact]
        public async Task GetAll_WithLargeDataset_Should_ReturnCorrectly()
        {
            var emptyList = new List<Category>();
          
            var testCategories = HelperTestMethod.CreateListTestCategories( _applicationUser,_output,15);
            var service = GetGetAllCategoriesUnderTest(testCategories);

            var result = await service.GetAllCategoriesAsync();

            var assert = new AssertApiHelpers(_output);

            assert.ShouldSucceed(result, "Successfully retrieved all categories.");            
            
        }
        [Fact]
        public async Task GetAll_WithUnauthorizedUser_Should_ReturnUnauthorizedError()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid().ToString();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        
            // Set up invalid user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, invalidUserId)
            };
            
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"))
            };
            
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        
            // Create in-memory database with no users
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            using var dbContext = new ApplicationDbContext(options);
            
            var service = new GetAllCategoriesService(
                dbContext,
                new Mock<ILogger<GetAllCategoriesService>>().Object,
                mockHttpContextAccessor.Object);
        
            // Act
            var result = await service.GetAllCategoriesAsync();
        
            // Assert
            var assert = new AssertApiHelpers(_output);
            assert.ShouldFail(result, "User profile not found.");
        }
    }
}
*/
