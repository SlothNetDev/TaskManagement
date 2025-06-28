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
namespace TaskManagement.Infrastructures.Services.TaskService
{
    public class UpdateTaskService(ILogger<UpdateTaskService> _logger,
        TaskManagementDbContext _dbContext,
        IHttpContextAccessor _httpContextAccessor) : IUpdateTaskService
    {
        public async Task<ResponseType<TaskResponseDto>> UpdateTaskAsync(TaskUpdateDto request)
        {
            //add response
            var response = new ResponseType<TaskResponseDto>();

            //1. validate user request 
            var validationErrors = ModelValidation.ModelValidationResponse(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}",
               "POST /login",
               validationErrors);
                response.Success = false;
                response.Message = "Field Request for Models has an Error";
                return response;
            }

            //2. Get User Id and check who are you
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //check if User id was empty or fake 
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid parseId))
            {
                _logger.LogWarning("No category found for user id {userId}.",userId);
                response.Success = false;
                response.Message = "Unauthorized or invalid user.";
                return response;
            }

            //3. Validate if task has category
            var hasCategory = await _dbContext.CategoryDb.AnyAsync(x => x.UserId == parseId);
            //check if your account has any category
            if (!hasCategory)
            {
                _logger.LogWarning("No category found for user.");
                response.Success = false;
                response.Message = "No category found for the user. Please create a category first.";
                return response;
            }

            

            //4. get User Id
            var updateTask = await _dbContext.TaskDb.FirstOrDefaultAsync(x => x.UserId == parseId);
            //check if id was exist
            if(updateTask is null)
            {
                _logger.LogWarning($"{updateTask} Id Cannot Found");
                response.Success = false;
                response.Message = "Blog not found";
                return response;
            }
            try
            {
                //update field
                updateTask.Title = request.Title ?? string.Empty;
                updateTask.Priority = request.Priority ?? Priority.Low; // default as low
                updateTask.Status = request.Status ?? Status.InProgress; // default as inProgress
                updateTask.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "Task Update Successfully";
                response.Data = new TaskResponseDto(updateTask.Id, updateTask.Title,
                    updateTask.Priority.ToString(), updateTask.Status.ToString() ?? string.Empty, updateTask.DueDate, 
                    updateTask.CreatedAt,updateTask.UpdatedAt);
                return response;

            }catch(Exception ex)
            {
                _logger.LogInformation("Task Update Failed from user {user}, Reason: {reason}", updateTask.Id,ex.Message);
                response.Success = false;
                response.Message = "Failed to Update Task";
                return response;
            }

        }
    }
}
