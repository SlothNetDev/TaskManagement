using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.Categories.Query
{
    public class GetAllCategoriesService(TaskManagementDbContext _dbContext,
        ILogger<GetAllCategoriesService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IGetAllCategories
    {
        public async Task<ResponseType<List<CategoryResponseDtoWithTask>>> GetAllCategoriesAsync()
        {
            //create response
            var response = new ResponseType<List<CategoryResponseDtoWithTask>>();

            //get user id
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                 _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userId);
                response.Success = false;
                response.Message = "Unauthorized or invalid user.";
                return response;
            }
            try
            {
                //query the category with task
                var categoryQuery = _dbContext.CategoryDb
                    .Where(t => t.UserId == parseUserId)
                    .Include(t => t.Tasks)
                    .Select(c => new CategoryResponseDtoWithTask(c.Id,
                              c.CategoryName,
                    c.Description ?? string.Empty,
                        c.Tasks.Select(task => new TaskResponseDto(task))
                        .ToList()));

                var query = await categoryQuery.ToListAsync();

                if (categoryQuery.Any())
                {
                    _logger.LogInformation("No Category found for user {UserId}", parseUserId);
                    response.Success = false;
                    response.Message = "You have no Category. Create one first.";
                    return response;
                }
                response.Success = true;
                response.Message = "Successfully Display All Categories";
                response.Data = query;
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Categories for user.");
                response.Success = false;
                response.Message = "An error occurred while retrieving Categories.";
                return response;
            }
        }
    }
}
