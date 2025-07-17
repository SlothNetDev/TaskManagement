using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IGetDomainTaskRepository
{
    Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateTaskAsync();
    Task<ResponseType<TaskItem>> GetCurrentUserDomainIdUpdateTaskAsync(Guid id);
}