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
    public class TokenService(
    JwtSettings _settings,
    TaskManagementDbContext _dbContext,
    IHttpContextAccessor _httpContextAccessor) : ITokenService
    {
        public async Task<AuthResultDto> GenerateTokenAsync(TokenUserDto user)
        {
            var IpAddress = new IPadressHelper(_httpContextAccessor);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
    
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
    
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
            var jwtToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.UtcNow.AddMinutes(15),
                claims: claims,
                signingCredentials: creds);
    
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshTokenDto = GenerateRefreshToken(user.UserId, IpAddress.GetIpAddress());
    
            var refreshToken = MapToEntity(refreshTokenDto, Guid.Parse(user.UserId));
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
    
            return new AuthResultDto(
                tokenString,
                jwtToken.ValidTo,
                refreshTokenDto.Token,
                user.UserName,
                string.Join(",", user.Roles));
        }
    
        public RefreshTokenResponseDto GenerateRefreshToken(string userId, string ipAddress)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var created = DateTime.UtcNow;
    
            return new RefreshTokenResponseDto(
                Guid.NewGuid().ToString(),
                token,
                created.AddDays(7),
                false,
                created,
                ipAddress,
                null,
                null,
                true
            );
        }
    
        private RefreshToken MapToEntity(RefreshTokenResponseDto dto, Guid userId) =>
            new()
            {
                Id = dto.Id,
                Token = dto.Token,
                Expires = dto.Expires,
                Created = dto.Created,
                CreatedByIp = dto.CreatedByIp,
                UserId = userId,
                Revoked = dto.Revoked,
                RevokedByIp = dto.RevokedByIp
            };
    
        

      
    }


        
    

}
