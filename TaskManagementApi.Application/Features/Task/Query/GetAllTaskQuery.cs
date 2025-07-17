using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record GetAllTaskQuery : IRequest<ResponseType<List<TaskResponseDto>>>;

    public class GetAllTaskQueryHandler(ITaskRepository dbContext,
        ILogger<CreateTaskCommandHandler> logger,
        IGetDomainTaskRepository identityService) : IRequestHandler<GetAllTaskQuery, ResponseType<List<TaskResponseDto>>>
    {
        public async Task<ResponseType<List<TaskResponseDto>>> Handle(GetAllTaskQuery request, CancellationToken cancellationToken)
        {
            var taskResponse = await identityService.GetCurrentUserDomainIdGetAllTaskAsync();
            if (!taskResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", taskResponse.Message);
                return ResponseType<List<TaskResponseDto>>.Fail(taskResponse.Message);
            }
            var userDomain = taskResponse.Data;
            try
            {
                var task = await dbContext.GetAllTaskAsync(userDomain);

                var taskDto = task
                    .Select(t => new TaskResponseDto(
                        t.Id,
                        t.Title,
                        t.Priority.ToString() ?? string.Empty,
                        t.Status.ToString() ?? string.Empty,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt))
                    .ToList();
                if (!taskDto.Any())
                {
                    logger.LogInformation("GT_INFO: No tasks found for user {UserId}", userDomain);
                    return ResponseType<List<TaskResponseDto>>.SuccessResult(
                        new List<TaskResponseDto>(),
                        "No tasks found. Create your first task!");
                }

                logger.LogInformation("GT_SUCCESS: Retrieved {TaskCount} tasks for user {UserId}",
                    taskDto.Count, userDomain);

                return ResponseType<List<TaskResponseDto>>.SuccessResult(
                    taskDto,
                    $"Found {taskDto.Count} tasks");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GT_ERROR: Failed to retrieve tasks for user {UserId}", userDomain);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    ex.Message,
                    "Failed to retrieve tasks. Please try again");
            }
        }
    }
}
