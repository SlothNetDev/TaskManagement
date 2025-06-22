using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Interfaces
{
    /// <summary>
    /// For Logging and Register
    /// </summary>
    public interface ILoginCommand
    {
        Task<ResponseType<UserResponseDto>> LoginAsync(UserLoginRequestDto loginDto);
    }
}
