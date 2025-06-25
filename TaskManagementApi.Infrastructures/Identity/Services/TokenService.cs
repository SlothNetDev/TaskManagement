using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Identity.Services
{
    public class TokenService(JwtSettings _settings) : ITokenService
    {
        public RefreshTokenResponseDto GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = "127.0.0.1"
            };
            return new RefreshTokenResponseDto(
            refreshToken.Id,
            refreshToken.Token,
            refreshToken.Expires,
            refreshToken.IsExpired,
            refreshToken.Created,
            refreshToken.CreatedByIp,
            refreshToken.Revoked,
            refreshToken.RevokedByIp,
            refreshToken.IsActive
        );
        }

        public Task<AuthResultDto> GenerateTokenAsync(TokenUserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.UtcNow.AddDays(7),
                claims: claims,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var result = new AuthResultDto(
                tokenString,
                DateTime.UtcNow.AddMinutes(15),
                user.UserName,
                user.Roles.ToString() ?? string.Empty);

            return Task.FromResult(result);
        }


        
    }

}
