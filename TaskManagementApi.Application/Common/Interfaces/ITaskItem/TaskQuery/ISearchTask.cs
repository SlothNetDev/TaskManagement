using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.TaskDto;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery
{
    public interface ISearchTask
    {
        Task<List<TaskResponseDto>> SearchTaskAsync();
    }
}
