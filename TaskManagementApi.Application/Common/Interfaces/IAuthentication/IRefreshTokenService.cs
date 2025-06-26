using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;
using static TaskManagementApi.Application.Features.Authentication.DTOs.UserDto;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IRefreshTokenService
    {
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
        Task<ResponseType<List<RefreshTokenResponseDto>>> GetRefreshTokenAsync(Guid userId);

        /// <summary>
        /// Logs the user out by revoking the refresh token.
        /// </summary>
        Task<ResponseType<string>> LogoutAsync(LogOutRequestDto dto);
    }
}
