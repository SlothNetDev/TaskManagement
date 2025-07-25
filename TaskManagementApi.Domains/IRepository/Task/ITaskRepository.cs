using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Core.IRepository.Task;

public interface ITaskRepository
{
    Task<TaskItem> CreateAsync(TaskItem taskItem);
    Task<TaskItem> UpdateAsync(TaskItem taskItem);
    Task<TaskItem> DeleteAsync(TaskItem taskItemId);
    Task<TaskItem> GetByIdAsync(TaskItem taskItemId);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> PaganationAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task<IEnumerable<TaskItem>> ISearchTask(string searchTerm);
    
}