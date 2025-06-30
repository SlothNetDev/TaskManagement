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
        TaskManagementDbContext _dbContext,
        ILogger<DeleteCategoryService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IDeleteCategoryService
    {
        public async Task<ResponseType<CategoryResponseDto>> DeleteCategoryAsync(Guid Id)
        {
            var response = new ResponseType<CategoryResponseDto>();      

            //Get user Jwt
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userId);
                response.Success = false;
                response.Message = "Unauthorized or invalid user.";
                return response;
            }

             // macth the applicationUsert to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);

             //4. Create Category
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            //5. combine and main Id
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            var deleteCategory = await _dbContext.CategoryDb.FirstOrDefaultAsync(d => d.UserId == taskUserIdToUse && d.UserId == Id);

            if(deleteCategory is null)
            {
                _logger.LogWarning("No task found with ID {Id} for user {UserId}.", Id, parseUserId);
                response.Success = false;
                response.Message = "Task not found.";
                return response;
            }
            try
            {
                _dbContext.Remove(deleteCategory);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully  Deleted {Id}");
                response.Success = true;
                response.Data = new CategoryResponseDto(deleteCategory);
                return response;
            }
            catch(Exception ex)
            {
                 _logger.LogError("Error Deleting Category with Id {Id}, Reason: {message}",Id,ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
