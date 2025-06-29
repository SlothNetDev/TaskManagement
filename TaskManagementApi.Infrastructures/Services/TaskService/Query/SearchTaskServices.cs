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
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("Invalid or unauthorized user.");
                response.Success = false;
                response.Message = "Unauthorized.";
                return response;
            }
    
            // 2. Prepare query
            var query = _dbContext.TaskDb
                .Include(t => t.Category)
                .Where(x => x.UserId == parsedUserId);
    
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
