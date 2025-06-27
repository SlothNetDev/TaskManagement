using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ITask.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;
namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record CreateTaskCommand(TaskRequestDto createDto) : IRequest<ResponseType<TaskResponseDto>>;
    public class CreateTaskCommandHandler(ICreateTask task) : IRequestHandler<CreateTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            return await task.CreateTaskAsync(request.createDto);
        }
    }
}
