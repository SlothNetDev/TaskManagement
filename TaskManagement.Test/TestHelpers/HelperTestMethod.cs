/*
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Entities;
using Xunit.Abstractions;

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
        public static ApplicationUsers CreateTestUser(Guid userId)
        {
            return new ApplicationUsers
            {
                Id = userId,
                DomainUserId = Guid.NewGuid()
            };
        }
        public static List<Category> CreateListTestCategories(ApplicationUsers _applicationUser, ITestOutputHelper _output,int count = 1)
        {
            var category =  Enumerable.Range(1, count).Select(i => new Category
            {
                Id = Guid.NewGuid(),
                UserId = _applicationUser.DomainUserId,
                CategoryName = $"Category {i}",
                Description = $"Description {i}",
                Tasks = new List<TaskItem>()
            }).ToList();

            _output.WriteLine("List of Categories: \n");
            foreach(var list in category)
            {
                _output.WriteLine($"id: {list.Id}," +
                    $"UserId: {list.UserId}," +
                    $"CategoryName: {list.CategoryName}," +
                    $"Description: {list.Description}," +
                    $"Task: {list.Tasks.ToList()}");
            }
            return category;
        }
        public static Category CreateTestCategories(ApplicationUsers _applicationUser,ITestOutputHelper _output,CategoryRequestDto request = null)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                UserId = _applicationUser.DomainUserId,
                CategoryName = request.CategoryName,
                Description = request.Description,
                Tasks = new List<TaskItem>()
            };

            _output.WriteLine("List of Categories: \n");

            _output.WriteLine($"id: {category.Id}," +
                $"UserId: {category.UserId}," +
                $"CategoryName: {category.CategoryName}," +
                $"Description: {category.Description}," +
                $"Task: {category.Tasks.ToList()}");

            return category;
        }


    }
}
*/
