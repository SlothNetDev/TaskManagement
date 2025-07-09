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
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService.Command
{
    public class DeleteTaskService(
        ApplicationDbContext _dbContext,
        ILogger<DeleteTaskService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IDeleteTaskService
    {
        public async Task<ResponseType<TaskResponseDto>> DeleteTaskAsync(Guid taskId)
        {
            // 1. Validate task ID
            if (taskId == Guid.Empty)
            {
                _logger.LogWarning("DT_001: Attempted to delete task with empty ID");
                return ResponseType<TaskResponseDto>.Fail(
                    "Invalid task identifier",
                    "Task ID cannot be empty");
            }
        
            // 2. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("DT_002: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
                return ResponseType<TaskResponseDto>.Fail(
                    "Authentication failed",
                    "Invalid user credentials");
            }
        
            // 3. Match application user
            _logger.LogInformation("DT_DEBUG: Attempting task deletion for UserId: {UserId}", parsedUserId);
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("DT_003: No user profile found for ID {UserId}", parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "User profile not found",
                    "Invalid user account");
            }
        
            // 4. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("DT_004: Empty domain ID for user {UserId}", parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "User configuration incomplete",
                    "Missing domain user ID");
            }
        
            // 5. Find and validate task
            var taskToDelete = await _dbContext.TaskDb
                .FirstOrDefaultAsync(t => t.UserId == taskUserIdToUse && t.Id == taskId);
        
            if (taskToDelete == null)
            {
                _logger.LogWarning("DT_005: Task {TaskId} not found for user {UserId}", taskId, parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "Task not found",
                    "The specified task doesn't exist or you don't have permission");
            }
        
            // 6. Delete task
            try
            {
                _dbContext.Remove(taskToDelete);
                await _dbContext.SaveChangesAsync();
        
                _logger.LogInformation("DT_SUCCESS: Deleted task {TaskId} ('{TaskTitle}') for user {UserId}", 
                    taskId, taskToDelete.Title, parsedUserId);
        
                return ResponseType<TaskResponseDto>.SuccessResult(
                    new TaskResponseDto(taskToDelete),
                    $"Successfully deleted task: {taskToDelete.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DT_ERROR: Failed to delete task {TaskId}", taskId);
                return ResponseType<TaskResponseDto>.Fail(
                    ex.Message,
                    "Failed to delete task. Please try again");
            }
        }
    }
}
