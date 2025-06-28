using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService
{
    public class GetAllTaskService(IHttpContextAccessor _httpContextAccessor,
        TaskManagementDbContext _dbContext,
        ILogger<GetAllTaskService> _logger) : IGetAllTask
    {
        public async Task<ResponseType<List<TaskResponseDto>>> GetAllTaskAsync()
        {
            var response = new ResponseType<List<TaskResponseDto>>();

             // 1. Validate user Id
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("Invalid or unauthorized user.");
                response.Success = false;
                response.Message = "Unauthorized.";
                return response;
            }

            try
            {
                //2. Query Tasks for this user
                var userTasks = await _dbContext.TaskDb
                    .Where(ac => ac.UserId == parsedUserId)
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TaskResponseDto(t.Id, t.Title,
                    t.Priority.ToString(),
                    t.Status.ToString() ?? string.Empty,
                    t.CreatedAt, t.DueDate, t.UpdatedAt))
                    .ToListAsync();

                if (!userTasks.Any())
                {
                    _logger.LogInformation("No tasks found for user {UserId}", parsedUserId);
                    response.Success = false;
                    response.Message = "You have no tasks. Create one first.";
                    return response;
                }

                //3. return response result
                response.Success = true;
                response.Data = userTasks;
                response.Message = "Successfully Display all Blogs";
                return response;  
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve tasks for user.");
                response.Success = false;
                response.Message = "An error occurred while retrieving tasks.";
                return response;
            }
        }
    }
}
