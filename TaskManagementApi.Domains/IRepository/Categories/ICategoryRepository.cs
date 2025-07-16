using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Core.IRepository.Categories;

/// <summary>
/// Represents data Access logic for managing CategoryCreateRepository
/// </summary>
public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<Category> DeleteAsync(Category categoryId);
    Task<Category> UpdateAsync(Category category);
    Task<List<Category>> GetByAllCategoryAsync(Guid id);
    Task<Category> GetByIdAsync(Guid id);
}