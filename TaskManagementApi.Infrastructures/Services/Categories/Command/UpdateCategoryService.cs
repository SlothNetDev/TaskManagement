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
            var response = new ResponseType<CategoryResponseDto>();

            //1. Validate user
            var validationErrors = ModelValidation.ModelValidationResponse(requestDto)
            ?.Where(e => !string.IsNullOrWhiteSpace(e))
            ?.ToList();


            //check if all field  request was correct
            if (validationErrors?.Count > 0)
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
            //check id categories if exist
            var updateCategory = await _dbContext.CategoryDb.FirstOrDefaultAsync(id => id.UserId == taskUserIdToUse && id.Id == requestDto.Id);
            //check if categoryId is exist
            if(updateCategory is null)
            {
                _logger.LogDebug("Looking for Category with UserId: {UserId}, CategoryId: {CategoryId}", taskUserIdToUse, requestDto.Id);
                response.Success = false;
                response.Message = "Category not found";
                return response;
            }

            try
            {
                //update category
                updateCategory.CategoryName = requestDto.CategoryName ?? string.Empty;
                updateCategory.Description = requestDto.Description ?? string.Empty;

                //save the update
                await _dbContext.SaveChangesAsync();
                response.Success = true;
                response.Data = new CategoryResponseDto(
                    updateCategory.Id,
                    updateCategory.CategoryName,
                    updateCategory.Description);
                response.Message = "Category Updated Successfully";
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Categories Update Failed from user {user}, Reason: {reason}", updateCategory.Id,ex.Message);
                response.Success = false;
                response.Message = "Failed to Update Categories";
                return response;
            }
        }
    }
}
