using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IIdentityService
    {
        Task<AuthResultDto> LoginAsync(UserLoginRequestDto dto);
        Task<AuthResultDto> RegisterAsync(UserRegisterRequestDto dto);
    }
}
