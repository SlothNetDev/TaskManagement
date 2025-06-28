using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record GetAllTaskQuery : IRequest<ResponseType<TaskResponseDto>>;

    public class GetAllTaskQueryHandler(IGetAllTask getTask) : IRequestHandler<GetAllTaskQuery, ResponseType<TaskResponseDto>>
    {
        public async Task<ResponseType<TaskResponseDto>> Handle(GetAllTaskQuery request, CancellationToken cancellationToken)
        {
            return await getTask.GetAllTaskAsync();
        }
    }
}
