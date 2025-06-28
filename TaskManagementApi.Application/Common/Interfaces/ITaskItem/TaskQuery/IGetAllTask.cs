using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery
{
    public interface IGetAllTask
    {
        Task<ResponseType<List<TaskResponseDto>>> GetAllTaskAsync();
    }
}
