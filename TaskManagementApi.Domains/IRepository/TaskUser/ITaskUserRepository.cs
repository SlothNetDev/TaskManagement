using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Core.IRepository.User;

public interface ITaskUserRepository
{
    Task<TaskUsers> GetApplicationUserIdAysnc();
    
    
}