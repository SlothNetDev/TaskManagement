using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IGetDomainIdCategoryRepository
{
    /// <summary>
    /// Get User Id 
    /// </summary>
    /// <returns>Guid User Id</returns>
    Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateCategoryAsync();
    Task<ResponseType<Category>> GetCurrentUserDomainIdUpdateCategoryAsync(Guid id);
    Task<ResponseType<Category>> GetCurrentUserDomainIdDeleteCategoryAsync(Guid id);
    Task<ResponseType<Guid>> GetCurrentUserDomainIdGetAllCategoryAsync();
    Task<ResponseType<Category>> GetCurrentUserDomainIdGetByIdCategoryAsync(Guid id);
    
}