
using System.Security.Claims;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    /// <summary>
    /// For JWT generating Token
    /// </summary>
    public interface ITokenService
    {
        Task<AuthResultDto> GenerateTokenAsync(TokenUserDto user); 
        RefreshTokenResponseDto GenerateRefreshToken();
    }
}
