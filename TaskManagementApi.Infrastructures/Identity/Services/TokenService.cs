using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.InfrustructureHelper;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Identity.Services
{
    public class TokenService(JwtSettings _settings,
    ApplicationDbContext _dbContext,
    IHttpContextAccessor _httpContextAccessor) : ITokenService
    {
        public async Task<AuthResultDto> GenerateTokenAsync(TokenUserDto user)
        {
            var ipAddress = GetIpAddress();
    
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
    
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
    
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            //create jwt 
            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                claims: claims,
                signingCredentials: creds);
    
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = GenerateRefreshToken(user.UserId, ipAddress);
            
            // Store refresh token
            await _dbContext.RefreshTokens.AddAsync(new RefreshToken
            {
                Id = refreshToken.Id,
                Token = refreshToken.Token,
                Created = refreshToken.Created,
                CreatedByIp = ipAddress,
                Expires = refreshToken.Expires,
                UserId = Guid.Parse(user.UserId)
            });

            await _dbContext.SaveChangesAsync();

            return new AuthResultDto
            {
                BearerToken = token,
                ExpiresAt = jwt.ValidTo,
                RefreshToken = refreshToken.Token,
                UserName = user.UserName,
                Role = string.Join(",", user.Roles)
            };
        }
    
        public RefreshTokenResponseDto GenerateRefreshToken(string userId, string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress,
                UserId = Guid.Parse(userId)
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
        public RefreshToken GenerateRefreshTokenEntity(string userId, string ipAddress)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress,
                UserId = Guid.Parse(userId)
            };
        }

        private string GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }
    }



        
    

}
