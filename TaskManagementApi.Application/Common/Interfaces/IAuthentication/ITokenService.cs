
using System.Security.Claims;
using TaskManagement.Infrastructures.Identity.Models;
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
        Task<string> GenerateTokenAsync(TokenUserDto user);
        Task<ResponseType<AuthResultDto>> RefreshTokenAsync(string token, string refreshToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
