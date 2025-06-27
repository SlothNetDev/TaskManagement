using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record UpdateTaskCommand(TaskUpdateDto dto) :IRequest<ResponseType<TaskResponseDto>>;
    public class UpdateTaskCommandHandler(IUpdateTaskService service) : IRequestHandler<UpdateTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            return await service.UpdateTaskAsync(request.dto);
        }
    }
}
