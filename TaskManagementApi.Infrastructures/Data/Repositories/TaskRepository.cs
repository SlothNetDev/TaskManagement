using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data.Repositories;

public class TaskRepository(ApplicationDbContext dbContext) :ITaskRepository
{
    public async Task<TaskItem> CreateAsync(TaskItem taskItem)
    {
        await dbContext.TaskDb.AddAsync(taskItem);
        await dbContext.SaveChangesAsync();
        return taskItem;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem taskItem)
    {
        dbContext.TaskDb.Update(taskItem);
        await dbContext.SaveChangesAsync();
        return taskItem;
    }

    public async Task<TaskItem> DeleteAsync(TaskItem taskItemId)
    {
        dbContext.TaskDb.Remove(taskItemId);
        await dbContext.SaveChangesAsync();
        return taskItemId;
    }

    public async Task<TaskItem> GetByIdAsync(Guid id)
    {
        var task =  await dbContext.TaskDb.FirstOrDefaultAsync(x => x.Id == id);
        return new TaskItem
        {
            Id = task.Id,
            CategoryId = task.CategoryId,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt ?? null,
        };
    }

    public async Task<IEnumerable<TaskItem>> GetAllTaskAsync(Guid id)
    {
        return await Task.FromResult<IEnumerable<TaskItem>>(new List<TaskItem>());
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