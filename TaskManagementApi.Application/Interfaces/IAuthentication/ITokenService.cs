using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Interfaces
{
    /// <summary>
    /// For JWT generating Token
    /// </summary>
    public interface ITokenService
    {
        Task<ResponseType<string>> GenerateTokenAsync(ApplicationUsers user);
    }
}
