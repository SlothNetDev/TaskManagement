using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.ITask.TaskCommand;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Enums;
using TaskManagementApi.Domains.Wrapper;
namespace TaskManagement.Infrastructures.Services.TaskService.Command
{
    public class UpdateTaskService(ILogger<UpdateTaskService> _logger,
        ApplicationDbContext _dbContext,
        IHttpContextAccessor _httpContextAccessor) : IUpdateTaskService
    {
       public async Task<ResponseType<TaskResponseDto>> UpdateTaskAsync(TaskUpdateDto request)
        {
            // 1. Validate request
            var validationErrors = ModelValidation.ModelValidationResponse(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("UT_001: Request validation failed. Errors: {@ValidationErrors}", 
                    validationErrors);
                return ResponseType<TaskResponseDto>.Fail(
                    validationErrors,
                    "Invalid task data. Please correct the highlighted fields");
            }
        
            // 2. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("UT_002: Invalid user token {UserId}", userId);
                return ResponseType<TaskResponseDto>.Fail(
                    "Authentication failed",
                    "Invalid user credentials");
            }
        
            // 3. Match application user
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("UT_003: User profile not found {UserId}", parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "User profile not found",
                    "Please complete your account setup");
            }
        
            // 4. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("UT_004: Missing domain ID for user {UserId}", parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "Account configuration incomplete",
                    "Missing domain user ID");
            }
        
            // 5. Verify user has at least one category
            var hasCategory = await _dbContext.CategoryDb
                .AnyAsync(x => x.UserId == taskUserIdToUse);
            
            if (!hasCategory)
            {
                _logger.LogWarning("UT_005: No categories found for user {UserId}", parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "No categories available",
                    "Please create a category first");
            }
        
            // 6. Find and validate task
            var taskToUpdate = await _dbContext.TaskDb
                .FirstOrDefaultAsync(x => x.UserId == taskUserIdToUse && x.Id == request.Id);
        
            if (taskToUpdate == null)
            {
                _logger.LogWarning("UT_006: Task {TaskId} not found for user {UserId}", 
                    request.Id, parsedUserId);
                return ResponseType<TaskResponseDto>.Fail(
                    "Task not found",
                    "The specified task doesn't exist or you don't have permission");
            }
        
            try
            {
                // 7. Apply updates
                taskToUpdate.Title = request.Title ?? taskToUpdate.Title;
                taskToUpdate.Priority = request.Priority ?? taskToUpdate.Priority;
                taskToUpdate.Status = request.Status ?? taskToUpdate.Status;
                taskToUpdate.UpdatedAt = DateTime.UtcNow;
        
                await _dbContext.SaveChangesAsync();
        
                _logger.LogInformation("UT_SUCCESS: Updated task {TaskId} for user {UserId}", 
                    request.Id, parsedUserId);
        
                return ResponseType<TaskResponseDto>.SuccessResult(
                    new TaskResponseDto(
                        taskToUpdate.Id,
                        taskToUpdate.Title,
                        taskToUpdate.Priority.ToString(),
                        taskToUpdate.Status.ToString(),
                        taskToUpdate.DueDate,
                        taskToUpdate.CreatedAt,
                        taskToUpdate.UpdatedAt),
                    "Task updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UT_ERROR: Failed to update task {TaskId}", request.Id);
                return ResponseType<TaskResponseDto>.Fail(
                    ex.Message,
                    "Failed to update task. Please try again");
            }
        }
    }
}
