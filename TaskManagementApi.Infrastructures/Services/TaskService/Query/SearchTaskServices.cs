using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService.Query
{
    public class SearchTaskServices(ApplicationDbContext _dbContext,
     ILogger<SearchTaskServices> _logger,
     IHttpContextAccessor _httpContextAccessor) : ISearchTask 
    {
        public async Task<ResponseType<List<TaskResponseDto>>> SearchTaskAsync(string search)
        {
            // 1. Validate User
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("SRCH_TASK_001: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "Unauthorized.",
                    "Authentication failed. Invalid user identifier."
                );
            }
    
            // Match the applicationUser to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
    
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("SRCH_TASK_002: No matching ApplicationUser found for userId {ParsedUserId}.", parsedUserId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "UserNotFound", // A specific error code for internal use if desired
                    "User profile not found."
                );
            }
    
            // Combine
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("SRCH_TASK_003: User {ParsedUserId} has an empty DomainUserId.", parsedUserId);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "DomainIdNotConfigured",
                    "User domain ID is not configured."
                );
            }
    
            try
            {
                // 2. Prepare query
                var query = _dbContext.TaskDb
                    .Include(t => t.Category)
                    .Where(x => x.UserId == taskUserIdToUse);
    
                var lowerSearch = search?.ToLower().Trim();
    
                if (!string.IsNullOrWhiteSpace(lowerSearch))
                {
                    query = query.Where(x =>
                        x.Title.ToLower().Contains(lowerSearch) ||
                        x.Status.ToString().ToLower().Contains(lowerSearch) ||
                        x.Priority.ToString().ToLower().Contains(lowerSearch) ||
                        (x.DueDate.HasValue && x.DueDate.Value.ToString("yyyy-MM-dd").ToLower().Contains(lowerSearch)) || // More specific date format
                        (x.Category != null && x.Category.CategoryName.ToLower().Contains(lowerSearch)) // Null check for category
                    );
                }
    
                var taskResponse = await query
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TaskResponseDto(
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString() ?? string.Empty,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt))
                    .ToListAsync();
    
                string message;
                if (!string.IsNullOrWhiteSpace(lowerSearch))
                {
                    message = taskResponse.Any()
                        ? $"Successfully found {taskResponse.Count} tasks matching '{search}'."
                        : "No tasks found matching your search criteria.";
                }
                else
                {
                    message = "Successfully retrieved all tasks.";
                }
    
                // Using the SuccessResult factory method
                return ResponseType<List<TaskResponseDto>>.SuccessResult(taskResponse, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SRCH_TASK_004: An unexpected error occurred while searching tasks for user {ParsedUserId} with search term '{SearchTerm}'.", parsedUserId, search);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "InternalServerError", // A general error code
                    "An unexpected error occurred while searching for tasks. Please try again later."
                );
            }
        }
    }

}
