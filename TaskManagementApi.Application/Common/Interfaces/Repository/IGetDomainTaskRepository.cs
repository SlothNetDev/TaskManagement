using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IGetDomainTaskRepository
{
    Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateTaskAsync();
    Task<ResponseType<TaskItem>> GetCurrentUserDomainIdUpdateTaskAsync(Guid id);
    Task<ResponseType<TaskItem>> GetCurrentUserDomainIdDeleteTaskAsync(Guid id);
    
    
    Task<ResponseType<Guid>> GetCurrentUserDomainIdGetAllTaskAsync();
    Task<ResponseType<TaskItem>> GetCurrentUserDomainIdGetByIdTaskAsync(Guid id);
    Task<ResponseType<Guid>> GetCurrentUserDomainIdPaganationTaskAsync(CancellationToken cancellationToken);
    Task<ResponseType<TaskItem>> GetCurrentUserDomainIdSearchTaskAsync(string id);
    
    
}