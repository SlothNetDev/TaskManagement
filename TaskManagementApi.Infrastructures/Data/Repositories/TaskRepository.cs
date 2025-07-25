using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data.Repositories;

public class TaskRepository(ApplicationDbContext dbContext) :ITaskRepository
{
    public async Task<TaskItem> CreateAsync(TaskItem taskItem)
    {
        await dbContext.AddAsync(taskItem);
        await dbContext.SaveChangesAsync();
        return taskItem;
    }

    public Task<TaskItem> UpdateAsync(TaskItem taskItem)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem> DeleteAsync(TaskItem taskItemId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem> GetByIdAsync(TaskItem taskItemId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> PaganationAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> ISearchTask(string searchTerm)
    {
        throw new NotImplementedException();
    }
}