using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Common.Interfaces.ITask.TaskCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.Categories.Command
{
    public class CreateCategoryService(
        ApplicationDbContext _dbContext,
        ILogger<CreateCategoryService> _logger,
        IHttpContextAccessor _httpContextAccessor) : ICreateCategoryService
    {
        public async Task<ResponseType<CategoryResponseDto>> CreateCategoryAsync(CategoryRequestDto requestDto)
        {
            // 1. Validate request
            var validationErrors = ModelValidation.ModelValidationResponse(requestDto);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}", 
                    "POST /category", 
                    validationErrors);
                return ResponseType<CategoryResponseDto>.Fail(validationErrors, "Invalid input. Please check the provided data");
            }
        
            // 2. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
                return ResponseType<CategoryResponseDto>.Fail("Unauthorized or invalid user.");
            }
        
            // 3. Match application user
            _logger.LogInformation("Attempting to create category for UserId: {UserId}", parseUserId);
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
                return ResponseType<CategoryResponseDto>.Fail("Invalid user account");
            }
        
            // 4. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
                return ResponseType<CategoryResponseDto>.Fail("Invalid user configuration");
            }
        
            // 5. Create and save category
            try
            {
                var category = new Category
                {
                    UserId = taskUserIdToUse,
                    Id = Guid.NewGuid(),
                    CategoryName = requestDto.CategoryName,
                    Description = requestDto.Description
                };
        
                await _dbContext.AddAsync(category);
                await _dbContext.SaveChangesAsync();
        
                _logger.LogInformation("Category {categoryId} created successfully for user {userId}", 
                    category.Id, taskUserIdToUse);
        
                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(category),
                    "Category created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category for user {userId}", taskUserIdToUse);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message, 
                    "Failed to create category");
            }
        }
    }
}
