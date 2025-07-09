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
            // 1. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("GA_CAT_001: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(
                    "Authentication failed. Invalid user identifier.");
            }
        
            // 2. Match application user
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("GA_CAT_002: No matching ApplicationUser found for userId {ParsedUserId}.", parsedUserId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(
                    "User profile not found.");
            }
        
            // 3. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("GA_CAT_003: User {ParsedUserId} has an empty DomainUserId.", parsedUserId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(
                    "User domain ID is not configured.");
            }
        
            try
            {
                // 4. Query categories with tasks
                var categories = await _dbContext.CategoryDb
                    .Where(c => c.UserId == taskUserIdToUse)
                    .Include(c => c.Tasks)
                    .Select(c => new CategoryResponseDtoWithTask(
                        c.Id,
                        c.CategoryName,
                        c.Description ?? string.Empty,
                        c.Tasks.Select(t => new TaskResponseDto(t)).ToList()))
                    .ToListAsync();
        
                if (!categories.Any())
                {
                    _logger.LogInformation("GA_CAT_004: No Categories found for user {ParsedUserId}.", parsedUserId);
                    return ResponseType<List<CategoryResponseDtoWithTask>>.SuccessResult(
                        new List<CategoryResponseDtoWithTask>(),
                        "No categories found. Consider creating one.");
                }
        
                _logger.LogInformation("Successfully retrieved {Count} categories for user {UserId}", 
                    categories.Count, parsedUserId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.SuccessResult(
                    categories,
                    "Successfully retrieved all categories.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GA_CAT_005: Failed to retrieve categories for user {ParsedUserId}", parsedUserId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(
                    ex.Message,
                    "Failed to retrieve categories. Please try again later.");
            }
        }
    }

    
}
