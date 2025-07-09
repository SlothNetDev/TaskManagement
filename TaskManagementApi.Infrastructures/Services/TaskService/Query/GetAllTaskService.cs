using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService.Query
{
    public class GetAllTaskService(IHttpContextAccessor _httpContextAccessor,
        ApplicationDbContext _dbContext,
        ILogger<GetAllTaskService> _logger) : IGetAllTask
    {
       public async Task<ResponseType<List<TaskResponseDto>>> GetAllTaskAsync()
        {
            // 1. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("GT_001: Invalid authentication token for user ID: {UserId}", userId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "Authentication failed",
                    "Invalid user credentials");
            }
        
            // 2. Match application user
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("GT_002: User profile not found for ID: {UserId}", parsedUserId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "User profile not found",
                    "Please complete your account setup");
            }
        
            // 3. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("GT_003: Missing domain ID for user: {UserId}", parsedUserId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "Account configuration incomplete",
                    "Missing domain user ID");
            }
        
            try
            {
                // 4. Query tasks
                var tasks = await _dbContext.TaskDb
                    .Where(t => t.UserId == taskUserIdToUse)
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TaskResponseDto(
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt))
                    .ToListAsync();
        
                if (!tasks.Any())
                {
                    _logger.LogInformation("GT_INFO: No tasks found for user {UserId}", parsedUserId);
                    return ResponseType<List<TaskResponseDto>>.SuccessResult(
                        new List<TaskResponseDto>(),
                        "No tasks found. Create your first task!");
                }
        
                _logger.LogInformation("GT_SUCCESS: Retrieved {TaskCount} tasks for user {UserId}", 
                    tasks.Count, parsedUserId);
                
                return ResponseType<List<TaskResponseDto>>.SuccessResult(
                    tasks,
                    $"Found {tasks.Count} tasks");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GT_ERROR: Failed to retrieve tasks for user {UserId}", parsedUserId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    ex.Message,
                    "Failed to retrieve tasks. Please try again");
            }
        }
    }
}
