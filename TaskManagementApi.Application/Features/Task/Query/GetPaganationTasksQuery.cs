using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Core.Wrapper;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record GetPaganationTasksQuery(PaganationDto dto) :IRequest<ResponseType<PaganationResponse<TaskResponseDto>>>;

    public class GetPaganationTasksQueryHandler(ITaskRepository dbContext,
        ILogger<CreateTaskCommandHandler> logger,
        IGetDomainTaskRepository identityService) : IRequestHandler<GetPaganationTasksQuery, ResponseType<PaganationResponse<TaskResponseDto>>>
    {
        public async Task<ResponseType<PaganationResponse<TaskResponseDto>>> Handle(GetPaganationTasksQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate pagination request
            if (request.dto.PageNumber < 1 || request.dto.PageSize < 1)
            {
                logger.LogWarning("PAG_001: Invalid pagination request - Page: {Page}, Size: {Size}", 
                    request.dto.PageNumber, request.dto.PageSize);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Invalid pagination parameters",
                    "Page number and size must be positive integers");
            }

            var taskResponse = await identityService.GetCurrentUserDomainIdPaganationTaskAsync(cancellationToken);
            if (!taskResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", taskResponse.Message);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(taskResponse.Message);
            }
            var userDomain = taskResponse.Data;
            try
            {
                var (items, totalCount) = await dbContext.GetPaginatedTasksAsync(
                    userDomain, request.dto.PageNumber, request.dto.PageSize);

                var taskDtos = items.Select(t => new TaskResponseDto(
                    t.Id,
                    t.Title,
                    t.Priority.ToString(),
                    t.Status.ToString(),
                    t.DueDate,
                    t.CreatedAt,
                    t.UpdatedAt
                )).ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)request.dto.PageSize);
                logger.LogInformation(
                    "PAG_SUCCESS: Retrieved {TaskCount} tasks (page {Page} of {TotalPages}) for user {UserId}",
                    taskDtos.Count, request.dto.PageNumber, totalPages, userDomain);

                return ResponseType<PaganationResponse<TaskResponseDto>>.SuccessResult(
                    new PaganationResponse<TaskResponseDto>
                    {
                        Data = taskDtos,
                        TotalRecords = totalCount,
                        PageSize = request.dto.PageNumber,
                        CurrentPage = request.dto.PageNumber,
                    },
                    "Retrieved {taskDtos.Count} tasks");
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("PAG_CANCEL: Request cancelled for user {UserId}", userDomain);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    "Request cancelled",
                    "The operation was cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PAG_ERROR: Failed to retrieve paginated tasks for user {UserId}", userDomain);
                return ResponseType<PaganationResponse<TaskResponseDto>>.Fail(
                    ex.Message,
                    "Failed to retrieve tasks. Please try again");
            }
        }
    }
}
