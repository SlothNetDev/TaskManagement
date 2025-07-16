using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IGetDomainTaskRepository
{
    Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateTaskAsync();
    Task<ResponseType<Guid>> GetCurrentUserDomainIdUpdateTaskAsync(Guid id);
}