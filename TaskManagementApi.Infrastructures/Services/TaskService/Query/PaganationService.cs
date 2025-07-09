using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.Wrapper;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService.Query
{
    public class PaganationService(
        IHttpContextAccessor _httpContextAccessor,
        ApplicationDbContext _dbContext,
        ILogger<PaganationService> _logger) : IPaganationTaskService
    {
        public async Task<ResponseType<PaganationResponse<TaskResponseDto>>> PaganationAsync(PaganationDto request, CancellationToken cancellationToken)
        {
            // 1. Validate pagination request
            if (request.PageNumber < 1 || request.PageSize < 1)
            {
                _logger.LogWarning("PAG_001: Invalid pagination request - Page: {Page}, Size: {Size}", 
                    request.PageNumber, request.PageSize);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Invalid pagination parameters",
                    "Page number and size must be positive integers");
            }
        
            // 2. Get and validate user from JWT
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("PAG_002: Invalid authentication token for user ID: {UserId}", userId);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Authentication failed",
                    "Invalid user credentials");
            }
        
            // 3. Match application user
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId, cancellationToken);
        
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("PAG_003: User profile not found for ID: {UserId}", parsedUserId);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "User profile not found",
                    "Please complete your account setup");
            }
        
            // 4. Validate domain user ID
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("PAG_004: Missing domain ID for user: {UserId}", parsedUserId);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Account configuration incomplete",
                    "Missing domain user ID");
            }
        
            try
            {
                // 5. Get total count
                var totalCount = await _dbContext.TaskDb
                    .CountAsync(t => t.UserId == taskUserIdToUse, cancellationToken);
        
                // 6. Calculate pagination metadata
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
                
                // 7. Get paginated data
                var tasks = await _dbContext.TaskDb
                    .Where(t => t.UserId == taskUserIdToUse)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new TaskResponseDto(
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt))
                    .ToListAsync(cancellationToken);
        
                _logger.LogInformation("PAG_SUCCESS: Retrieved {TaskCount} tasks (page {Page} of {TotalPages}) for user {UserId}", 
                    tasks.Count, request.PageNumber, totalPages, parsedUserId);
        
                return ResponseType<PaganationResponse<TaskResponseDto>>.SuccessResult(
                    new PaganationResponse<TaskResponseDto>
                    {
                        Data = tasks,
                        TotalRecords = totalCount,
                        PageSize = request.PageSize,
                        CurrentPage = request.PageNumber,
                    },
                    $"Retrieved {tasks.Count} tasks");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PAG_CANCEL: Request cancelled for user {UserId}", parsedUserId);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Request cancelled",
                    "The operation was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PAG_ERROR: Failed to retrieve paginated tasks for user {UserId}", parsedUserId);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    ex.Message,
                    "Failed to retrieve tasks. Please try again");
            }
        }
            
    }
}
