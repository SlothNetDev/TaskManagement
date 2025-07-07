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
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Entities;
using Xunit.Abstractions;

namespace TaskManagement.Test.CategoryTest
{
    public class CreateCategory
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<ApplicationDbContext> _mockDbContext; //mock the db context of database
        private readonly Mock<ILogger<CreateCategoryService>> _mockLogger = new(); // mock category service
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new(); //mock httpContext

        private readonly CreateCategoryService _create_Service;

        private Guid _jwtUserId = Guid.NewGuid(); //jwt guid Id
        private Guid _domainUser = Guid.NewGuid(); // domain user TaskUser Id

        //set up for them to be accessible in test Fact
        private readonly CategoryRequestDto _validRequest;
        private readonly ApplicationUsers _applicationUser;

        public CreateCategory(ITestOutputHelper output)
        {
            _output = output;
            //create applicationUser
            _applicationUser = new ApplicationUsers
            {
                Id = _jwtUserId,
                DomainUserId = _domainUser
            };
            //create request category
             _validRequest = new CategoryRequestDto("My love life", "bwahalalaa");

            //make jwt token as valid token
            HttpContextAccessorHelperTest.SetUpHttpContextAccessor(_jwtUserId, _mockHttpContextAccessor);

            // Set up DbContextMock with stubbed data
            var dbContext = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockDbContext = dbContext;
            //create dbSet 
            dbContext.CreateDbSetMock(x => x.CategoryDb, new List<Category>());
            dbContext.CreateDbSetMock(x => x.UserApplicationDb, new[] { _applicationUser });

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            //assign it into create service
            _create_Service = new CreateCategoryService(_mockDbContext.Object,
                _mockLogger.Object, _mockHttpContextAccessor.Object);
        }

        /// <summary>
        /// check if create categories was success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateCategories()
        {
            AsserApiHelpers assert = new(_output);
            var result = await _create_Service.CreateCategoryAsync(_validRequest);

            assert.ShouldSucceed(result, "Category Created Successfully");
            assert.ShouldMatch(result, data =>
                data.CategoryName == _validRequest.CategoryName &&
                data.Description == _validRequest.Description);

            //Add and save changes
            _mockDbContext.Verify(db => db.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
