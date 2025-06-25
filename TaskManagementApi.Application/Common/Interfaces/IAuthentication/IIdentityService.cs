using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IIdentityService<TRefreshTokenDto>
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
        Task<ResponseType<AuthResultDto>> RegisterAsync(UserRegisterRequestDto dto);

        /// <summary>
        /// Generating Refresh Token for an Account
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns>AuthResponseDto </returns>
        Task<ResponseType<RefreshTokenResponseDto>> RefreshTokenAsync(string token, string refreshToken);
        /// <summary>
        /// Get all RefreshToken of Accounts
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of Account RefreshToken</returns>
        Task<ResponseType<List<TRefreshTokenDto>>> GetRefreshTokenAsync(Guid userId);
    }
}
