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
        public async Task<ResponseType<PaganationResponse<TaskResponseDto>>> PaganationAsync(PaganationDto request,CancellationToken cancellationToken)
        {
            var response = new ResponseType<PaganationResponse<TaskResponseDto>>();

             // 1. Validate user Id
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
            
            var totalCount = await _dbContext.TaskDb
                .CountAsync(t => t.UserId == taskUserIdToUse, cancellationToken);

            //Paganation 
            var task = await _dbContext.TaskDb
                .Where(t => t.UserId == parseUserId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TaskResponseDto(
                    t.Id,
                    t.Title,
                    t.Priority.ToString(),
                    t.Status.ToString() ?? string.Empty,
                    t.DueDate,
                    t.CreatedAt,
                    t.UpdatedAt))
                .ToListAsync();

            try
            {
                response.Success = true;
                response.Message = "Paginated task list retrieved successfully.";
                response.Data = new PaganationResponse<TaskResponseDto>
                {
                    Data = task,
                    TotalRecords = totalCount,
                    PageSize = request.PageSize,
                    CurrentPage = request.PageNumber
                };
                return response;
            }
            catch(Exception ex)
            {
                response.Success = true;
                response.Message = $"Paginated task list Failed, Reason: {ex.Message}.";
                return response;
            }


        }
    }
}
