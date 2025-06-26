using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IAuthService
    {
        /// <summary>
        /// Interface For Login Account Async
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>AuthResultDto response Type</returns>
        Task<ResponseType<AuthResultDto>> LoginAsync(UserLoginRequestDto dto);
        /// <summary>
        /// Interface for Register Account Async
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>AuthResult Dto Response Type</returns>
        Task<ResponseType<string>> RegisterAsync(UserRegisterRequestDto dto);
 
    }
}
