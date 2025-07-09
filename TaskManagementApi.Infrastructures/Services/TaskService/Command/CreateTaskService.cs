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
    public class CreateTaskService(ApplicationDbContext _dbContext,
        ILogger<CreateTaskService> _logger,
        IHttpContextAccessor _httpContextAccessor) : ICreateTask
    {
        public async Task<ResponseType<TaskResponseDto>> CreateTaskAsync(TaskRequestDto request)
        {

            //1. validate user request
            var validationErrors = ModelValidation.ModelValidationResponse(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}", 
                "POST /login", 
                validationErrors);
                return ResponseType<TaskResponseDto>.Fail("Field Request for Models has an Error");  
            }

            //2. Get user ID from Jwt
            var userIdJwt = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //validate userId
            if (string.IsNullOrWhiteSpace(userIdJwt) || !Guid.TryParse(userIdJwt, out var parseUserId))
            {
               _logger.LogWarning("Failed to extract valid user ID {id} from JWT.",userIdJwt);
               return ResponseType<TaskResponseDto>.Fail("Unauthorized or invalid user.");
            }
   
             //3 macth the applicationUsert to TaskUser(Domain)
            var matchingApplicationUser = await _dbContext.UserApplicationDb
                .FirstOrDefaultAsync(ac => ac.Id == parseUserId);

             //4. Create Category
            if (matchingApplicationUser == null)
            {
                _logger.LogWarning("No matching ApplicationUser found for userId {id}.", matchingApplicationUser.DomainUserId);
                return ResponseType<TaskResponseDto>.Fail("Unauthorized or invalid user");
            }
            //5. combine 
            var taskUserIdToUse = matchingApplicationUser.DomainUserId;
            if (taskUserIdToUse == Guid.Empty)
            {
                _logger.LogWarning("User {id} has an empty DomainUserId.", taskUserIdToUse);
                return ResponseType<TaskResponseDto>.Fail("Invalid User Account");
            }
            //3. Validate category if there was even category in user before task will created
            var hasCategory = _dbContext.CategoryDb.Any(x => x.UserId == taskUserIdToUse);
            if (hasCategory is false)
            {
                _logger.LogWarning("No category found for user.");
                return ResponseType<TaskResponseDto>.Fail("No category found for the user. Please create a category first.");
            }
            //get category if it was exist in user
            var category = await _dbContext.CategoryDb
                .AnyAsync(x => x.UserId == request.CategoryId);

            //validate category
            if (category is true)
            {
                _logger.LogError("Expected {Category} was null when processing {Create}",
                 category,
                 "Create Task");
                return ResponseType<TaskResponseDto>.Fail("Category don't exist to user Account");
            }

            //4. Create Task Item
            var createTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Priority = request.Priority,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UserId = taskUserIdToUse,
                CategoryId = request.CategoryId,
                DueDate = request.DueDate
            };

            //5. Save changes
            await _dbContext.AddAsync(createTask);
            await _dbContext.SaveChangesAsync();

            try
            {
                _logger.LogInformation("Task Created Successfully from user {user}", createTask.Id);
                return ResponseType<TaskResponseDto>.SuccessResult(new TaskResponseDto(createTask),"Task Created Successfully");
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Task Created Failed from user {user}, Reason: {reason}", createTask.Id,ex.Message);
                return ResponseType<TaskResponseDto>.Fail(new List<string> { ex.Message},"Failed to create task");
            }
        }
    }
}
