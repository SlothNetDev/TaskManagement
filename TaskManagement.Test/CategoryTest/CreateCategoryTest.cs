using Castle.Core.Logging;
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Services.Categories.Command;
using TaskManagement.Infrastructures.Services.Categories.Query;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Application.Features.CategoryFeature.Queries;
using TaskManagementApi.Domains.Entities;
using Xunit.Abstractions;

namespace TaskManagement.Test.CategoryTest
{
    public class CreateCategoryTest /*: IDisposable*/
    {
         private readonly ITestOutputHelper _output;
        private Mock<ApplicationDbContext> _mockDbContext;

        private Guid _jwtUserId = Guid.NewGuid();
        private readonly ApplicationUsers _applicationUser;
        public CreateCategoryTest(ITestOutputHelper output)
        {
            _output = output;
            //initialize test data
            _applicationUser = HelperTestMethod.CreateTestUser(_jwtUserId);
              

        }
        private Mock<ApplicationDbContext> GetMockDbContext(ApplicationUsers _applicationUser,Category _category)
        {
             // Set up DbContextMock
           var dbContext = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockDbContext = dbContext;
            dbContext.CreateDbSetMock(x => x.UserApplicationDb, new[] { _applicationUser });
            dbContext.CreateDbSetMock(x => x.CategoryDb, new[] {_category});
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            return dbContext;
        }
        private CreateCategoryService CreateCategoriesUnderTest(Category categories)
        {
             var _mockLogger = new Mock<ILogger<CreateCategoryService>>();
             var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            HelperTestMethod.SetUpHttpContextAccessor(_jwtUserId, _mockHttpContextAccessor);

            _mockDbContext = GetMockDbContext(_applicationUser, categories);

            return new CreateCategoryService(_mockDbContext.Object,
                _mockLogger.Object, _mockHttpContextAccessor.Object);
        }
            
        /// <summary>
        /// check if create categories was success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateCategories_WithValidData_Should_CreateSuccessfully()
        { 
            var dto = new CategoryRequestDto
            {
                CategoryName = "mahana",
                Description = "shogano"
            };
            var createCategory = HelperTestMethod.CreateTestCategories( _applicationUser, _output,dto);
            
            var service = CreateCategoriesUnderTest(createCategory);
            AsserApiHelpers assert = new(_output);
            //create request category

            var result = await service.CreateCategoryAsync(dto);

            assert.ShouldSucceed(result, "Category Created Successfully");
            assert.ShouldMatch(result, data =>
                data.CategoryName == result.Data.CategoryName &&
                data.Description == result.Data.Description);
        }
        [Fact]
        public async Task CreateEntity_Should_ReturnValidationError_WhenFieldExceedsMaxLength()
        {
            var dto = new CategoryRequestDto
            {
                CategoryName = new string('c',140),
                Description = new string('c',1500)
            };
            var createCategory = HelperTestMethod.CreateTestCategories( _applicationUser, _output,dto);
            
            var service = CreateCategoriesUnderTest(createCategory);
            AsserApiHelpers assert = new(_output);
            //create request category

            var result = await service.CreateCategoryAsync(dto);

            string expectedResponseMessage = "Invalid input. Please check the provided data";
            var expectedValidationErrors = new List<string>
            {
                "Category Name Cannot exceed with 120 characters",
                "Descriptions Cannot exceed with 1000 characters" // Adjust based on your actual Description error
            };

            assert.ShouldFail(result, expectedResponseMessage, expectedValidationErrors);
            
        }

        
    }
}
