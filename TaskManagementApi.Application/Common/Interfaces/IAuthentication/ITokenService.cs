using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    /// <summary>
    /// For JWT generating Token
    /// </summary>
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUsers user);
    }
}
