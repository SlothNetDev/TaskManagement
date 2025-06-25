
using System.Security.Claims;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Application.DTOs;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    /// <summary>
    /// For JWT generating Token
    /// </summary>
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(TokenUserDto user);
        RefreshToken GenerateRefreshToken(string ipAddress);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
