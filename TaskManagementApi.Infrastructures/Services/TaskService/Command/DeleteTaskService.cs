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
        TaskManagementDbContext _dbContext,
        ILogger<DeleteTaskService> _logger,
        IHttpContextAccessor _httpContextAccessor) : IDeleteTaskService
    {
        public async Task<ResponseType<TaskResponseDto>> DeleteTaskAsync(Guid id)
        {
            ResponseType<TaskResponseDto> response = new();

            //1.check if id was empty
            if(id == Guid.Empty)
            {
                 _logger.LogWarning($"{id} Cannot Empty");
                response.Success = false;
                response.Message = "Task Cannot Found";
                return response;
            }

             //2. Get User Id and check who are you
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //validate userId
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
            {
               _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userId);
               response.Success = false;
               response.Message = "Unauthorized or invalid user.";
               return response;
            }

             //3 macth the applicationUsert to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
                        // --- ADD THIS LOGGING ---
            _logger.LogInformation("Attempting to create task for UserId: {UserId}", parseUserId);
             //4. Create Category
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            //5. combine 
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", userId);
                response.Success = false;
                response.Message = "Invalid User.";
                return response;
            }
            //3. connect the user Id and Id that will user provide
            var taskToDelete = await _dbContext.TaskDb.FirstOrDefaultAsync(x => x.UserId == taskUserIdToUse && x.Id == id);
            if(taskToDelete is null)
            {
                _logger.LogWarning("No task found with ID {Id} for user {UserId}.", id, parseUserId);
                response.Success = false;
                response.Message = "Task not found.";
                return response;
            }
            try
            {
                _dbContext.Remove(taskToDelete);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully  Deleted {id}");
                response.Success = true;
                response.Message = $" Successfully  Deleted Task {taskToDelete.Title.ToString()}";
                response.Data = new TaskResponseDto(taskToDelete);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex} Error Deleting Task with Id {id}");
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
