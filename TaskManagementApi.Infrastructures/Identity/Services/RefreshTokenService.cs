using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.InfrustructureHelper;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Identity.Services
{
    public class RefreshTokenService(
    ApplicationDbContext _dbContext,
    UserManager<ApplicationUsers> _userManager,
    ILogger<RefreshTokenService> _logger,
    ITokenService _tokenService,
    IHttpContextAccessor _httpContextAccessor) : IRefreshTokenService
    {
        public async Task<ResponseType<RefreshTokenResponseDto>> RefreshTokenAsync(string token, string refreshToken)
        {
            var ipAddress = new IPadressHelper(_httpContextAccessor);
        
            var storedToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);
        
            if (storedToken == null || !storedToken.IsActive)
            {
                _logger.LogInformation("Refresh token {Token} is not Available", refreshToken);
                return ResponseType<RefreshTokenResponseDto>.Fail("Invalid or expired refresh token.");
            }
        
            storedToken.Revoked = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress.GetIpAddress();
            _dbContext.RefreshTokens.Update(storedToken);
        
            var roles = (await _userManager.GetRolesAsync(storedToken.User)).ToList();
        
            var newToken = await _tokenService.GenerateTokenAsync(new TokenUserDto(
                storedToken.UserId.ToString(),
                storedToken.User.UserName!,
                storedToken.User.Email!,
                roles
            ));
        
            await _dbContext.SaveChangesAsync();
        
            var dto = new RefreshTokenResponseDto(
                storedToken.Id,
                newToken.RefreshToken,
                storedToken.Expires,
                storedToken.IsExpired,
                storedToken.Created,
                storedToken.CreatedByIp,
                storedToken.Revoked,
                storedToken.RevokedByIp,
                storedToken.IsActive
            );
        
            return ResponseType<RefreshTokenResponseDto>.SuccessResult(dto, "Successfully Refresh The Token");
        }
        public async Task<ResponseType<List<RefreshTokenResponseDto>>> GetRefreshTokenAsync(Guid userId)
        {
            if (await _dbContext.RefreshTokens.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {_dbContext.RefreshTokens.Count()}");
                return ResponseType<List<RefreshTokenResponseDto>>
                    .Fail("No Blogs Found Found, Create One First");
            }
        
            try
            {
                var tokenAsQuery = await _dbContext.RefreshTokens
                    .Select(token => new RefreshTokenResponseDto(
                        token.Id, token.Token, token.Expires, token.IsExpired, token.Created, token.CreatedByIp,
                        token.Revoked, token.RevokedByIp, token.IsActive))
                    .ToListAsync();
        
                _logger.LogInformation("Successfully Display All Blogs");
        
                return ResponseType<List<RefreshTokenResponseDto>>
                    .SuccessResult(tokenAsQuery, "Successfully Display all Blogs");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex} Cannot Get All Blogs");
        
                return ResponseType<List<RefreshTokenResponseDto>>
                    .Fail(ex.Message, "Unexpected error occurred while retrieving refresh tokens.");
            }
        }

        public async Task<ResponseType<string>> LogoutAsync(LogOutRequestDto dto)
        {
            var ipAddress = new IPadressHelper(_httpContextAccessor);
        
            var token = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == dto.RefreshToken);
        
            if (token == null)
            {
                _logger.LogWarning("Attempt to logout with invalid token is null.");
                return ResponseType<string>.Fail("Token is Empty.", "Invalid logout request.");
            }
        
            if (token.IsExpired)
            {
                _logger.LogWarning("Attempt to logout with invalid, Token is expired.");
                return ResponseType<string>.Fail("Token was expired.", "Logout failed.");
            }
        
            if (token.Revoked != null)
            {
                _logger.LogWarning("Attempt to logout with invalid, token is revoked.");
                return ResponseType<string>.Fail("Token was revoked.", "Logout failed.");
            }
        
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress.GetIpAddress();
        
            _dbContext.Update(token);
            await _dbContext.SaveChangesAsync();
        
            _logger.LogInformation("Refresh token: {tokens} successfully revoked for logout.", token.Token);
            return ResponseType<string>.SuccessResult(null!, "Logout successful. Refresh token revoked.");
        }
        
    }
        

}
