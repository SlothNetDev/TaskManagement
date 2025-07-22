using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query;

public record GetByIdTaskQuery(Guid id) : IRequest<ResponseType<TaskResponseDto>>;

public class GetByIdTaskQueryHandler(ITaskRepository dbContext,
    ILogger<CreateTaskCommandHandler> logger,
    IGetDomainTaskRepository identityService) : IRequestHandler<GetByIdTaskQuery, ResponseType<TaskResponseDto>>
{
    public async Task<ResponseType<TaskResponseDto>> Handle(GetByIdTaskQuery request, CancellationToken cancellationToken)
    {
        var taskResponse = await identityService.GetCurrentUserDomainIdGetByIdTaskAsync(request.id);
        if (taskResponse.Success)
        {
            logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", taskResponse.Message);
            return ResponseType<TaskResponseDto>.Fail(taskResponse.Message);
        }
        var userDomain =  taskResponse.Data;
        try
        {
            var task = dbContext.GetByIdAsync(userDomain.UserId).Result;
            logger.LogInformation("Successfully Get task {categoryId}", userDomain.UserId);

            return ResponseType<TaskResponseDto>.SuccessResult(
                new TaskResponseDto(task),
                "Task retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving task {categoryId}", userDomain.UserId);
            return ResponseType<TaskResponseDto>.Fail(
                ex.Message,
                "Failed to delete Task");
        }
    }
}