using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.ITask.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;
using TaskManagementApi.Domains.Enums;
namespace TaskManagement.Infrastructures.Services.TaskService.Command
{
    public class CreateTaskService(TaskManagementDbContext _dbContext,
        ILogger<CreateTaskService> _logger,
        IHttpContextAccessor _httpContextAccessor) : ICreateTask
    {
        public async Task<ResponseType<TaskResponseDto>> CreateTaskAsync(TaskRequestDto request)
        {
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

            //2. Get user ID from Jwt
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //validate userId
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseId))
            {
               _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userId);
               response.Success = false;
               response.Message = "Unauthorized or invalid user.";
               return response;
            }

            //3. Validate category if there was even category in user before task will created
            var hasCategory = _dbContext.CategoryDb.Any(x => x.UserId == parseId);
            if (!hasCategory)
            {
                _logger.LogWarning("No category found for user.");
                response.Success = false;
                response.Message = "No category found for the user. Please create a category first.";
                return response;
            }

            //get category if it was exist in user
            var category = await _dbContext.CategoryDb
                .FirstOrDefaultAsync(x => x.UserId == parseId);

            //validate category
            if(category is null)
            {
                _logger.LogError("Expected {Category} was null when processing {Create}", 
                 category, 
                 "Create Task");
                response.Success = false;
                response.Message = "Category don't exist to user Account";
                return response;
            }

            //4. Create Task Item
            var createTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Priority = request.Priority,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UserId = parseId,
                CategoryId = category.Id,
                DueDate = request.DueDate
            };

            //5. Save changes
            await _dbContext.AddAsync(createTask);
            _dbContext.SaveChanges();

            try
            {
                _logger.LogInformation("Task Created Successfully from user {user}", createTask.Id);
                response.Success = true;
                response.Message = "Task Created Successfully";
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Task Created Failed from user {user}, Reason: {reason}", createTask.Id,ex.Message);
                response.Success = false;
                response.Message = "Failed to Create Task";
                return response;
            }
        }
    }
}
