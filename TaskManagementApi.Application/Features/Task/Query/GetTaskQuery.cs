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
    public record GetAllTaskQuery : IRequest<ResponseType<List<TaskResponseDto>>>;

    public class GetAllTaskQueryHandler(IGetAllTask getTask) : IRequestHandler<SearchTaskQuery, ResponseType<List<TaskResponseDto>>>
    {
        public async Task<ResponseType<List<TaskResponseDto>>> Handle(SearchTaskQuery request, CancellationToken cancellationToken)
        {
            return await getTask.GetAllTaskAsync();
        }
    }
}
