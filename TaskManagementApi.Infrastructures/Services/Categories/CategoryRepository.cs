using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.Categories
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
            throw new NotImplementedException();
        }

        public Task<Category> UpdateAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetByAllCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
