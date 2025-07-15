using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Core.IRepository.Categories;

/// <summary>
/// Represents data Access logic for managing CategoryCreateRepository
/// </summary>
public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<Category> DeleteAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<List<Category>> GetByAllCategoryAsync(int id);
    Task<Category> GetByIdAsync(int id);
}