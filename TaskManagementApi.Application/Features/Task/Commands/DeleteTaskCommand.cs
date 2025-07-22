using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record DeleteTaskCommand(Guid id) : IRequest<ResponseType<TaskResponseDto>>;

    public class DeleteTaskCommandHandler(ITaskRepository dbContext,
        ILogger<CreateTaskCommandHandler> logger,
        IGetDomainTaskRepository identityService) : IRequestHandler<DeleteTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            //get jwt user Id
            var userDomainResponse = await identityService.GetCurrentUserDomainIdDeleteTaskAsync(request.id);
            if (!userDomainResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", userDomainResponse.Message);
                return ResponseType<TaskResponseDto>.Fail(userDomainResponse.Message);
            }
            var taskToDelete = userDomainResponse.Data;
            try
            {
                await dbContext.DeleteAsync(taskToDelete);
        
                logger.LogInformation("DT_SUCCESS: Deleted task {TaskId} ('{TaskTitle}') for user {UserId}", 
                    taskToDelete, taskToDelete.Title, taskToDelete.UserId);
        
                return ResponseType<TaskResponseDto>.SuccessResult(
                    new TaskResponseDto(taskToDelete),
                    $"Successfully deleted task: {taskToDelete.Title}");
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category {categoryId}", taskToDelete);
                return ResponseType<TaskResponseDto>.Fail(
                    ex.Message,
                    "Failed to delete category");
            }
        }
    }
}
