using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery
{
    public interface ISearchTask
    {
        Task<ResponseType<List<TaskResponseDto>>> SearchTaskAsync();
    }
}
