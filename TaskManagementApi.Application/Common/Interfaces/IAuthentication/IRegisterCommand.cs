using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IRegisterCommand
    {
        Task<ResponseType<AuthResultDto>> RegisterAsync(UserRegisterRequestDto registerDto);
    }
}
