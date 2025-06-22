using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Interfaces.IAuthentication
{
    public interface IRegisterCommand
    {
        Task<ResponseType<UserResponseDto>> RegisterAsync(UserRegisterRequestDto registerDto, string role);
    }
}
