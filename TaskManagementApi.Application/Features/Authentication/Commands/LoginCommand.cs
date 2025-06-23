using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Data;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Application.Interfaces;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    /// <summary>
    /// Login Account Service
    /// </summary>
    public class LoginCommand : ILoginCommand
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        public LoginCommand(ITokenService tokenService, UserManager<ApplicationUsers> userManager, RoleManager<ApplicationRole> roleManager,ILogger logger)
        {
            _logger = logger;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<ResponseType<AuthResultDto>> LoginAsync(UserLoginRequestDto loginDto)
        {
            var response = new ResponseType<AuthResultDto>();

            //0. Check if information by user is correct
            var validateUser = ModelValidation.ModelValidationResponse(loginDto);

            if (validateUser.Any()) //If any error will catch
            {
                //logging if there's something wrong in fields by user
                 _logger.LogWarning("Validation Failed for {Email}. Invalid fields: {@validationError}",
                     loginDto.Email,validateUser);

                 response.Success = false;
                 response.Message = "There are Model Validation Occurred";
                 return response;
            }

            //1. finding email if it's exist 
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null)
            {
                _logger.LogError("Login attempt for non-existent email: {Email}", loginDto.Email);
                response.Success = false;
                response.Message = "Invalid login credentials." + $"User {loginDto.Email} not found.";
                return response;
            }
            //2. check if password was correct by built in method
            var passwordValid = await _userManager.CheckPasswordAsync(user,loginDto.Password);

            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password attempt for user {UserId} (Email: {Email})", 
                user.Id, loginDto.Email);

                response.Success = false;
                response.Message = "Invalid login credentials." + $"{loginDto.Password} is incorrect";
                return response;
            }
            // 3. Get the roles assigned to the user (could be one or more)
            var roles = await _userManager.GetRolesAsync(user);

            // For this simple example, we'll just use the first role (if there's more than one)
            var userRole = roles.FirstOrDefault() ?? "User";

            //4. generate token a JWT token for user
            var token = await _tokenService.GenerateTokenAsync(user);

            //5. create a expiration date
            DateTime expireAt = DateTime.UtcNow.AddYears(1);
            try
            {
                _logger.LogInformation("User {UserId} logged in successfully. Role: {UserRole}", user.Id, userRole);
                response.Success = true;
                response.Message = "Successfully Login your Account";
                response.Data = new AuthResultDto(token, expireAt, user.UserName ?? string.Empty , userRole);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Login failed for account. Reason: {ErrorMessage}", ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
