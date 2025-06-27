using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITask
{
    public interface ICreateTask
    {
        Task<ResponseType<TaskResponseDto>> CreateTaskAsync(TaskRequestDto request);
    }
}
