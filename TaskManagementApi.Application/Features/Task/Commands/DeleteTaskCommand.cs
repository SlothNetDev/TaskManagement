using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Commands
{
    public record DeleteTaskCommand(Guid id) : IRequest<ResponseType<TaskResponseDto>>;

    public class DeleteTaskCommandHandler(IDeleteTaskService deleteTask) : IRequestHandler<DeleteTaskCommand, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            return await deleteTask.DeleteTaskAsync(request.id);
        }
    }
}
