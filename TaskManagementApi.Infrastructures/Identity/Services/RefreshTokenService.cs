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
    TaskManagementDbContext _dbContext,
    UserManager<ApplicationUsers> _userManager,
    ILogger<RefreshTokenService> _logger,
    ITokenService _tokenService,
    IHttpContextAccessor _httpContextAccessor) : IRefreshTokenService
    {
        public async Task<ResponseType<RefreshTokenResponseDto>> RefreshTokenAsync(string token, string refreshToken)
        {
            var response = new ResponseType<RefreshTokenResponseDto>();
    
            var storedToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);
    
            if (storedToken == null || !storedToken.IsActive)
            {
                _logger.LogInformation("Refresh token {Token} is not Available", refreshToken);
                response.Message = "Invalid or expired refresh token.";
                return response;
            }
    
            storedToken.Revoked = DateTime.UtcNow;
            storedToken.RevokedByIp = "127.0.0.1"; // Optional: Capture actual IP
            _dbContext.RefreshTokens.Update(storedToken);
    
            var roles = (await _userManager.GetRolesAsync(storedToken.User)).ToList();
    
            var newToken = await _tokenService.GenerateTokenAsync(new TokenUserDto(
                storedToken.UserId.ToString(),
                storedToken.User.UserName!,
                storedToken.User.Email!,
                roles
            ));
    
            await _dbContext.SaveChangesAsync();
    
            response.Message = "Successfully Refresh The Token";
            response.Data = (new RefreshTokenResponseDto(
                storedToken.Id,
                newToken.RefreshToken, // Return new refresh token only
                storedToken.Expires,
                storedToken.IsExpired,
                storedToken.Created,
                storedToken.CreatedByIp,
                storedToken.Revoked,
                storedToken.RevokedByIp,
                storedToken.IsActive));
            
            return response;
    
        }
    
        public Task<ResponseType<List<RefreshTokenResponseDto>>> GetRefreshTokenAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseType<string>> LogoutAsync(LogOutRequestDto dto)
        {
            var IpAddress = new IPadressHelper(_httpContextAccessor);
            var response = new ResponseType<string>();

            //Finding the refresh Token in database first
            var token = _dbContext.RefreshTokens
                .FirstOrDefault(x => x.Token == dto.Token);

            if(token == null || token.IsExpired || token.Revoked != null)
            {
                _logger.LogWarning("Attempt to logout with invalid or already revoked refresh token.");
                response.Success = false;
                response.Message = "Invalid or already revoked refresh token.";
                return response;
            }

            //Mark the token as Revoked
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = IpAddress.GetIpAddress();

            //Update to database
            _dbContext.Update(token);
            await _dbContext.SaveChangesAsync();

             _logger.LogInformation("Refresh token: {tokens} successfully revoked for logout.",token.Token);
            response.Success = true;
            response.Message = "Logout successful. Refresh token revoked.";
            return response;
        }
    }

}
