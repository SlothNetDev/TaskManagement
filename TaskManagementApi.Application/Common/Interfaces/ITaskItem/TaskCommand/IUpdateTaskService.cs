using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand
{
    public interface IUpdateTaskService
    {
        Task<ResponseType<TaskResponseDto>> UpdateTaskAsync(TaskUpdateDto request);
    }
}
