using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.Categories.Command
{
    public class DeleteCategoryService(
        ApplicationDbContext _dbContext,
        ILogger<DeleteCategoryService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IDeleteCategoryService
    {
       public async Task<ResponseType<CategoryResponseDto>> DeleteCategoryAsync(Guid categoryId)
        {
            // 1. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
                return ResponseType<CategoryResponseDto>.Fail("Unauthorized or invalid user.");
            }
        
            // 2. Match application user
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
                return ResponseType<CategoryResponseDto>.Fail("Invalid user account.");
            }
        
            // 3. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
                return ResponseType<CategoryResponseDto>.Fail("Invalid user configuration.");
            }
        
            // 4. Find and validate category
            var categoryToDelete = await _dbContext.CategoryDb
                .FirstOrDefaultAsync(c => c.UserId == taskUserIdToUse && c.Id == categoryId);
        
            if (categoryToDelete == null)
            {
                _logger.LogWarning("No category found with ID {categoryId} for user {userId}.", 
                    categoryId, parseUserId);
                return ResponseType<CategoryResponseDto>.Fail("Category not found.");
            }
        
            // 5. Delete category
            try
            {
                _dbContext.Remove(categoryToDelete);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deleted category {categoryId}", categoryId);
                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(categoryToDelete),
                    "Category deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {categoryId}", categoryId);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message,
                    "Failed to delete category");
            }
        }
    }
}
