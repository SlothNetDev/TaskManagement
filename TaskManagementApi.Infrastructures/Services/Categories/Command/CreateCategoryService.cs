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
        TaskManagementDbContext _dbContext,
        ILogger<CreateCategoryService> _logger,
        IHttpContextAccessor _httpContextAccessor) : ICreateCategoryService
    {
        public async Task<ResponseType<CategoryResponseDto>> CreateCategoryAsync(CategoryRequestDto requestDto)
        {
            var response = new ResponseType<CategoryResponseDto>();

            //1. Validate user
            var validationErrors = ModelValidation.ModelValidationResponse(requestDto);

            //check if all field  request was correct
            if (validationErrors.Any())
            {
                _logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}", 
                "POST /login", 
                validationErrors);
                response.Success = false;
                response.Message = "Field Request for Models has an Error";
                return response;
            };

            //2. Get user for JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userId);
                response.Success = false;
                response.Message = "Unauthorized or invalid user.";
                return response;
            }

            //3 macth the applicationUsert to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);

            // --- ADD THIS LOGGING ---
            _logger.LogInformation("Attempting to create category for UserId: {UserId}", parseUserId);
             //4. Create Category
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            //5. combine 
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }

            var category = new Category
            {
                UserId = taskUserIdToUse,
                Id = Guid.NewGuid(),
                CategoryName = requestDto.CategoryName,
                Description = requestDto.Description
            };
            await _dbContext.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            try
            {
                _logger.LogInformation("Category Created Successfully from user {user}", category.Id);
                response.Success = true;
                response.Message = "Category Created Successfully";
                response.Data = new CategoryResponseDto(category);                                                                              
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Category Created Failed from user {user}, Reason: {reason}", category.Id,ex.Message);
                response.Success = false;
                response.Message = "Failed to Create Category";
                return response;
            }
        }
    }
}
