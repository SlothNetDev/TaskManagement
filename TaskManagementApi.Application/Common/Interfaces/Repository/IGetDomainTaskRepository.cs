using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IGetDomainTaskRepository
{
    Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateCategoryAsync(Guid id);
    Task<ResponseType<Guid>> GetCurrentUserDomainIdUpdateCategoryAsync();
}