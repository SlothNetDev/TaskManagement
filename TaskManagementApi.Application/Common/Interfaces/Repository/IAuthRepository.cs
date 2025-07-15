using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.Repository;

public interface IAuthRepository
{
    /// <summary>
    /// Get User Id 
    /// </summary>
    /// <returns>Guid User Id</returns>
    Task<ResponseType<Guid>> GetCurrentUserDomainIdAsync();
    Task<ResponseType<Guid>> GetApplicationUserIdAndCheckCategoryExistAysnc(Guid id);
    
}