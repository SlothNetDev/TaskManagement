using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record UpdateTaskCommand(TaskUpdateDto dto) :IRequest<ResponseType<TaskResponseDto>>;
    public class UpdateTaskCommandHandler(ITaskRepository dbContext,
        ILogger<CreateTaskCommandHandler> logger,
        IGetDomainTaskRepository identityService) : IRequestHandler<UpdateTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            //1. validate user request
            var validationErrors = ModelValidation.ModelValidationResponse(request);
            if (validationErrors.Any())
            {
                logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}", 
                    "POST /login", 
                    validationErrors);
                return ResponseType<TaskResponseDto>.Fail("Field Request for Models has an Error");  
            }
            // 2. Get User Domain ID and validate Task non-existence
            var userDomainResponse = await identityService.GetCurrentUserDomainIdUpdateTaskAsync(request.dto.Id);

            if (!userDomainResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", userDomainResponse.Message);
                return ResponseType<TaskResponseDto>.Fail(userDomainResponse.Message);
            }

            var taskToUpdate = userDomainResponse.Data;
            try
            {
                taskToUpdate.Title = request.dto.Title ?? taskToUpdate.Title;
                taskToUpdate.Priority = request.dto.Priority ?? taskToUpdate.Priority;
                taskToUpdate.Status = request.dto.Status ?? taskToUpdate.Status;

                await dbContext.UpdateAsync(taskToUpdate);
                
                logger.LogInformation("UT_SUCCESS: Updated task {TaskId} for user {UserId}", 
                    request.dto.Id, taskToUpdate.UserId);

                return ResponseType<TaskResponseDto>.SuccessResult(
                    new TaskResponseDto(taskToUpdate),
                    "Task updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UT_ERROR: Failed to update task {TaskId}", request.dto.Id);
                return ResponseType<TaskResponseDto>.Fail(
                    ex.Message,
                    "Failed to update task. Please try again");
            }
            
            
        }   
    }
}
