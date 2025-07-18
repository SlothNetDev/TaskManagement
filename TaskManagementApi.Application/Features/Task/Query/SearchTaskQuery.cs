using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record SearchTaskQuery(string search) : IRequest<ResponseType<List<TaskResponseDto>>>;

    public class SearchTaskQueryHandler(ITaskRepository dbContext,
        ILogger<CreateTaskCommandHandler> logger,
        IGetDomainTaskRepository identityService) : IRequestHandler<SearchTaskQuery, ResponseType<List<TaskResponseDto>>>
    {
        public async Task<ResponseType<List<TaskResponseDto>>> Handle(SearchTaskQuery request, CancellationToken cancellationToken)
        { 
            var taskResponse = await identityService.GetCurrentUserDomainIdSearchTaskAsync(cancellationToken);
            if (!taskResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", taskResponse.Message);
                return ResponseType<List<TaskResponseDto>>.Fail(taskResponse.Message);
            }
            var userDomain = taskResponse.Data;
            
            try
            {
                var task =  await dbContext.ISearchTaskAsync(userDomain,request.search);
                
                var taskDto = task
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TaskResponseDto(
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString() ?? string.Empty,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt))
                    .ToList();
                string message;
                if (string.IsNullOrWhiteSpace(request.search))
                {
                    message = taskDto.Any()
                        ? $"Successfully found {taskDto.Count} tasks matching '{request.search}'."
                        : "No tasks found matching your search criteria.";
                }
                else
                {
                    message = "Successfully retrieved all tasks.";
                }
                return ResponseType<List<TaskResponseDto>>.SuccessResult(taskDto, message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SRCH_TASK_004: An unexpected error occurred while searching tasks for user {ParsedUserId} with search term '{SearchTerm}'.", userDomain,request.search);
                return ResponseType<List<TaskResponseDto>>.Fail(
                    "InternalServerError", // A general error code
                    "An unexpected error occurred while searching for tasks. Please try again later."
                );
            }
            
        }
    }
}
