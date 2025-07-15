using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Enums;
using TaskManagementApi.Domains.Wrapper;
namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record CreateTaskCommand(TaskRequestDto createDto) : IRequest<ResponseType<TaskResponseDto>>;
    public class CreateTaskCommandHandler(ITaskRepository task,
        ILogger<CreateTaskCommandHandler> logger,
        IAuthRepository identityService) : IRequestHandler<CreateTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(CreateTaskCommand request,
            CancellationToken cancellationToken)
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
            var userDomainReponse = await identityService.GetApplicationUserIdAndCheckCategoryExistAysnc(request.createDto.CategoryId);
            if (!userDomainReponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", userDomainReponse.Message);
                return ResponseType<TaskResponseDto>.Fail(userDomainReponse.Errors, userDomainReponse.Message);
            }
            var domainUserId = userDomainReponse.Data;
            
            //Create new Task
            var createTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.createDto.Title,
                Priority = request.createDto.Priority,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UserId = domainUserId,
                CategoryId = request.createDto.CategoryId,
                DueDate = request.createDto.DueDate
            };

            await task.CreateAsync(createTask);
            
            try
            {
                logger.LogInformation("Task Created Successfully from user {user}", createTask.Id);
                return ResponseType<TaskResponseDto>.SuccessResult(new TaskResponseDto(createTask),"Task Created Successfully");
            }
            catch(Exception ex)
            {
                logger.LogInformation("Task Created Failed from user {user}, Reason: {reason}", createTask.Id,ex.Message);
                return ResponseType<TaskResponseDto>.Fail(new List<string> { ex.Message},"Failed to create task");
            }
        }
        
    }
}
