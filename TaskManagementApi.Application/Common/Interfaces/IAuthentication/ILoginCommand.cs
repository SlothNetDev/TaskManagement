using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    /// <summary>
    /// For Logging and Register
    /// </summary>
    public interface ILoginCommand
    {
        Task<ResponseType<AuthResultDto>> LoginAsync(UserLoginRequestDto loginDto);
    }
}
