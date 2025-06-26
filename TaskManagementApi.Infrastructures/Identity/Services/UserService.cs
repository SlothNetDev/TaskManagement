using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Application.Common.Interfaces.IUser;
using TaskManagementApi.Application.Features.Users.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Identity.Services
{
    public class UserService(
    IHttpContextAccessor _httpContextAccessor,
    UserManager<ApplicationUsers> _userManager,
    ILogger<UserService> _logger) : IUserService
    {
        /// <summary>
        /// Gets the profile of the currently authenticated user
        /// </summary>
        public async Task<ResponseType<UserProfileDto>> UserProfileAsync()
        {
            ResponseType<UserProfileDto> response = new();
    
            // 1. Extract user ID from JWT claim
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated");
                response.Message = "User is not authenticated";
                response.Success = false;
                return response;
            }
    
            // 2. Query the user from database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Authenticated user not found in database: {UserId}", userId);
                response.Message = "User not found";
                response.Success = false;
                return response;
            }
    
            // 3. Get user roles
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
    
            // 4. Build and return profile DTO
            response.Success = true;
            response.Message = "Successfully retrieved user profile";
            response.Data = new UserProfileDto(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                roles
            );
    
            return response;
        }
    }

    
}
