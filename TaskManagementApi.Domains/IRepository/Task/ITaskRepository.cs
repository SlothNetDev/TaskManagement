using TaskManagementApi.Core.Wrapper;
using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Core.IRepository.Task;

public interface ITaskRepository
{
    Task<TaskItem> CreateAsync(TaskItem taskItem);
    Task<TaskItem> UpdateAsync(TaskItem taskItem);
    Task<TaskItem> DeleteAsync(TaskItem taskItemId);
    Task<TaskItem> GetByIdAsync(Guid taskItemId);
    Task<IEnumerable<TaskItem>> GetAllTaskAsync(Guid id);
    Task<(List<TaskItem> Items, int TotalCount)> GetPaginatedTasksAsync(Guid userId, int pageNumber, int pageSize);
    Task<IEnumerable<TaskItem>> ISearchTaskAsync(Guid id,string searchTerm);
    
}