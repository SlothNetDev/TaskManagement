using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.DTOs.UserDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Interfaces
{
    /// <summary>
    /// For Logging and Register
    /// </summary>
    public interface IAccountService
    {
        Task<ResponseType<UserResponseDto>> RegisterAsync(UserRegisterRequestDto registerDto, string role);
        Task<ResponseType<UserResponseDto>> LoginAsync(UserLoginRequestDto loginDto);
    }
}
