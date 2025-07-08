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
using TaskManagement.Infrastructures.Services.Categories.Command;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Test.HelperTest
{
    internal static class HelperTestMethod 
    {
        public static void SetUpHttpContextAccessor(Guid jwtUserID, Mock<IHttpContextAccessor> _mockHttpContextAccessor)
        {
            //convert jwtUserId into claims token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,jwtUserID.ToString())
            };
            //creating identity using token claims
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"))
            };
            //make httpAccessor to setUp the identity with fake token
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
                
        }
        public static Mock<ApplicationDbContext> CreateMockDbContext(ApplicationUsers user)
        {
            var dbContext = new DbContextMock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            
            dbContext.CreateDbSetMock(x => x.CategoryDb, new List<Category>());
            dbContext.CreateDbSetMock(x => x.UserApplicationDb, new[] { user });
            
            dbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
                   
            return dbContext;
        }
        public static ApplicationUsers CreateTestUser(Guid userId)
        {
            return new ApplicationUsers
            {
                Id = userId,
                DomainUserId = Guid.NewGuid()
            };
        }
    
    }
}
