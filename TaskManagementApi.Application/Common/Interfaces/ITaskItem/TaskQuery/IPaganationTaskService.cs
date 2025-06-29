using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.Wrapper;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery
{
    public interface IPaganationTaskService
    {
        Task<ResponseType<PaganationResponse<TaskResponseDto>>> PaganationAsync(PaganationDto request);
    }
}
