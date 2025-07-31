using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Core.Wrapper;
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

    public async Task<(List<TaskItem> Items, int TotalCount)> GetPaginatedTasksAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = dbContext.TaskDb.Where(t => t.UserId == userId);

        // Get total count
        var totalCount = await query.CountAsync();

        // Get paginated data
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<TaskItem>> ISearchTaskAsync(Guid taskUserIdToUse,string searchTerm)
    {
        // 2. Prepare query
        var query = dbContext.TaskDb
            .Include(t => t.Category)
            .Where(x => x.UserId == taskUserIdToUse);
    
        var lowerSearch = searchTerm?.ToLower().Trim();
    
        if (!string.IsNullOrWhiteSpace(lowerSearch))
        {
            query = query.Where(x =>
                    x.Title.ToLower().Contains(lowerSearch) ||
                    x.Status.ToString().ToLower().Contains(lowerSearch) ||
                    x.Priority.ToString().ToLower().Contains(lowerSearch) ||
                    (x.DueDate.HasValue && x.DueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ").ToLower().Contains(lowerSearch)) || // More specific date format
                    (x.Category != null && x.Category.CategoryName.ToLower().Contains(lowerSearch)) // Null check for category
            );
        }
        var queryResult = await query.ToListAsync();
        
        return queryResult;
    }
}