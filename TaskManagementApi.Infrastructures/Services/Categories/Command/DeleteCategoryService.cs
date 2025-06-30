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
using TaskManagementApi.Application.DTOs.TaskDto;
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

            var deleteCategory = await _dbContext.CategoryDb.FirstOrDefaultAsync(d => d.UserId == parseUserId && d.UserId == Id);

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
