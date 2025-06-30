using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.Wrapper;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Task.Query
{
    public record GetPaganationTasksQuery(PaganationDto dto,CancellationToken cancellationToken) :IRequest<ResponseType<PaganationResponse<TaskResponseDto>>>;

    public class GetPaganationTasksQueryHandler(IPaganationTaskService service) : IRequestHandler<GetPaganationTasksQuery, ResponseType<PaganationResponse<TaskResponseDto>>>
    {
        public Task<ResponseType<PaganationResponse<TaskResponseDto>>> Handle(GetPaganationTasksQuery request, CancellationToken cancellationToken)
        {
            return service.PaganationAsync(request.dto,request.cancellationToken);
        }
    }
}
