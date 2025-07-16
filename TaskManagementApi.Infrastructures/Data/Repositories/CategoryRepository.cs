using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Data.Repositories
{
    public class CategoryRepository(ApplicationDbContext dbContext ) :ICategoryRepository
    {
        public async Task<Category> CreateAsync(Category category)
        {
            await dbContext.CategoryDb.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return  category;
        }

        public async Task<Category> DeleteAsync(Category category)
        {
            dbContext.Remove(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            dbContext.Update(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetByAllCategoryAsync(Guid id)
        {
            return await dbContext.CategoryDb
                .Where(x => x.UserId == id)
                .Include(x => x.Tasks)
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await dbContext.CategoryDb
                .Where(x => x.UserId == id)
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync();
        }
    }
}
