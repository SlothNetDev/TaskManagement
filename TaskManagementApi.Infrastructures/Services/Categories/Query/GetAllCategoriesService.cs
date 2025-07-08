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
  
    public class GetAllCategoriesService(ApplicationDbContext _dbContext,
         ILogger<GetAllCategoriesService> _logger,
         IHttpContextAccessor _httpContextAccessor) : IGetAllCategories
    {
        public async Task<ResponseType<List<CategoryResponseDtoWithTask>>> GetAllCategoriesAsync()
        {
            //create response
            var response = new ResponseType<List<CategoryResponseDtoWithTask>>();
    
            //get user id
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("GA_CAT_001: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
                response.Success = false;
                response.Message = "Authentication failed. Invalid user identifier.";
                return response;
            }
    
            // match the applicationUser to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
    
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("GA_CAT_002: No matching ApplicationUser found for userId {ParsedUserId}.", parsedUserId);
                response.Success = false;
                response.Message = "User profile not found.";
                return response;
            }
    
            // combine
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("GA_CAT_003: User {ParsedUserId} has an empty DomainUserId.", parsedUserId);
                response.Success = false;
                response.Message = "User domain ID is not configured.";
                return response;
            }
    
            try
            {
                //query the category with task
                var categoryQuery = _dbContext.CategoryDb
                    .Where(t => t.UserId == taskUserIdToUse)
                    .Include(t => t.Tasks)
                    .Select(c => new CategoryResponseDtoWithTask(c.Id,
                                    c.CategoryName,
                                    c.Description ?? string.Empty,
                                    c.Tasks.Select(task => new TaskResponseDto(task))
                                    .ToList()));
    
                var query = await categoryQuery.ToListAsync();
    
                if (!query.Any())
                {
                    _logger.LogInformation("GA_CAT_004: No Categories found for user {ParsedUserId}.", parsedUserId);
                    response.Success = true; // Still a successful operation, just no data
                    response.Message = "No categories found for your account. Consider creating one.";
                    response.Data = new List<CategoryResponseDtoWithTask>(); // Return an empty list
                    return response;
                }
    
                response.Success = true;
                response.Message = "Successfully retrieved all categories.";
                response.Data = query;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GA_CAT_005: An unexpected error occurred while retrieving categories for user {ParsedUserId}.", parsedUserId);
                response.Success = false;
                response.Message = "An internal server error occurred while fetching categories. Please try again later.";
                return response;
            }
        }
    }

    
}
