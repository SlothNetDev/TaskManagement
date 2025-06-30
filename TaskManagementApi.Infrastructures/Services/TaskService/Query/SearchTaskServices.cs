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
    public class SearchTaskServices(
    ILogger<SearchTaskServices> _logger,
    IHttpContextAccessor _httpContextAccessor,
    TaskManagementDbContext _dbContext) : ISearchTask
    {
        public async Task<ResponseType<List<TaskResponseDto>>> SearchTaskAsync(string search)
        {
            var response = new ResponseType<List<TaskResponseDto>>();
    
            // 1. Validate User
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
                _logger.LogWarning("Invalid or unauthorized user.");
                response.Success = false;
                response.Message = "Unauthorized.";
                return response;
            }
             // macth the applicationUsert to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
            
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            // combine 
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            
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
                    (x.DueDate.HasValue && x.DueDate.Value.ToString().ToLower().Contains(lowerSearch)) ||
                    x.Category.CategoryName.ToLower().Contains(lowerSearch)
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
    
            response.Success = true;
            response.Data = taskResponse;
    
            if (!string.IsNullOrWhiteSpace(lowerSearch))
            {
                response.Message = taskResponse.Any()
                    ? $"{taskResponse.Count} tasks found matching '{search}'."
                    : "No tasks found matching the search criteria.";
            }
            else
            {
                response.Message = "All tasks retrieved successfully (no search term provided).";
            }
    
            return response;
        }
    }

}
