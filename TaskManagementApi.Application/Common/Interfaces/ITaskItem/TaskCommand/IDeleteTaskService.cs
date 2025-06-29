using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand
{
    public interface IDeleteTaskService
    {
        Task<ResponseType<TaskResponseDto>> DeleteTaskAsync(Guid id);
    }
}
