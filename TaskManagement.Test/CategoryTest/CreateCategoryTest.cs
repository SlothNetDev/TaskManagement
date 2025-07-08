using Castle.Core.Logging;
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Services.Categories.Command;
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
        private readonly Mock<ApplicationDbContext> _mockDbContext;

        private readonly ICreateCategoryService _categoriesService;

        private Guid _jwtUserId = Guid.NewGuid();

        private CategoryRequestDto _validRequest;
        private readonly ApplicationUsers _testUser = null!;
        public CreateCategoryTest(ITestOutputHelper output)
        {
            _output = output;

            //initialize test data
            _testUser = HelperTestMethod.CreateTestUser(_jwtUserId);
            //setUp mocks
            _mockDbContext = HelperTestMethod.CreateMockDbContext(_testUser);

            //httContextAccessor
            _categoriesService = CreateServiceUnderTest();
        }
        public CreateCategoryService CreateServiceUnderTest()
        {
            var mockLogger = new Mock<ILogger<CreateCategoryService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            
            HelperTestMethod.SetUpHttpContextAccessor(_jwtUserId, mockHttpContextAccessor);
            
            return new CreateCategoryService(
                _mockDbContext.Object,
                mockLogger.Object,
                mockHttpContextAccessor.Object
            );
        }
            
        /// <summary>
        /// check if create categories was success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateCategories_WithValidData_Should_CreateSuccessfully()
        {
            AsserApiHelpers assert = new(_output);
            //create request category
            _validRequest = new CategoryRequestDto
            {
                CategoryName = "My love life",
                Description = "bwahalalaa"
            };
            var result = await _categoriesService.CreateCategoryAsync(_validRequest);

            assert.ShouldSucceed(result, "Category Created Successfully");
            assert.ShouldMatch(result, data =>
                data.CategoryName == _validRequest.CategoryName &&
                data.Description == _validRequest.Description);
        }
        [Fact]
        public async Task CreateCatgories_WithBoundaryValues_Should_CreateSuccessfully()
        {
            AsserApiHelpers assert = new(_output);
            _validRequest = new CategoryRequestDto
            {
                CategoryName = new string('a', 120), //great value
                Description = new string('a', 1000) //exact value
            };
            var result = await _categoriesService.CreateCategoryAsync(_validRequest);

            assert.ShouldSucceed(result, "Category Created Successfully");
            assert.ShouldMatch(result, data =>
                data.CategoryName == _validRequest.CategoryName &&
                data.Description == _validRequest.Description);
        }

        
    }
}
