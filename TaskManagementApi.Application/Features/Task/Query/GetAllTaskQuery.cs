using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record GetAllTaskQuery : IRequest<ResponseType<List<TaskResponseDto>>>;

    public class GetAllTaskQueryHandler(IGetAllTask getTask) : IRequestHandler<GetAllTaskQuery, ResponseType<List<TaskResponseDto>>>
    {
        public async Task<ResponseType<List<TaskResponseDto>>> Handle(GetAllTaskQuery request, CancellationToken cancellationToken)
        {
            return await getTask.GetAllTaskAsync();
        }
    }
}
