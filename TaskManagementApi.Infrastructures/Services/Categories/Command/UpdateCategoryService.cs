using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.Categories.Command
{
    public class UpdateCategoryService(
        ApplicationDbContext _dbContext,
        ILogger<UpdateCategoryService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IUpdateCategoryService
    {
        public async Task<ResponseType<CategoryResponseDto>> UpdateCategoriesAsync(CategoryUpdateDto requestDto)
        {
            // 1. Validate request
            var validationErrors = ModelValidation.ModelValidationResponse(requestDto);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}",
                    "PUT /category",
                    validationErrors);
                return ResponseType<CategoryResponseDto>.Fail(validationErrors, "Invalid request data");
            }

            // 2. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
                return ResponseType<CategoryResponseDto>.Fail("Unauthorized or invalid user");
            }

            // 3. Match application user
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

            // 5. Find and validate category
            var categoryToUpdate = await _dbContext.CategoryDb
                .FirstOrDefaultAsync(c => c.UserId == taskUserIdToUse && c.Id == requestDto.Id);

            if (categoryToUpdate == null)
            {
                _logger.LogDebug("Category not found for UserId: {UserId}, CategoryId: {CategoryId}",
                    taskUserIdToUse, requestDto.Id);
                return ResponseType<CategoryResponseDto>.Fail("Category not found");
            }

            // 6. Update category
            try
            {
                // Apply updates (using null-coalescing for optional fields)
                categoryToUpdate.CategoryName = requestDto.CategoryName ?? categoryToUpdate.CategoryName;
                categoryToUpdate.Description = requestDto.Description ?? categoryToUpdate.Description;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Category {categoryId} updated successfully by user {userId}",
                    requestDto.Id, parseUserId);

                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(categoryToUpdate),
                    "Category updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category {categoryId}", requestDto.Id);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message,
                    "Failed to update category");
            }
        }
    
    }
}
