using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Identity.Services;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public class IdentityService(IOptions<IdentitySettings> _identitySettings,ILogger<IdentityService> _logger,
        UserManager<ApplicationUsers> _userManager,ITokenService _tokenService,ApplicationDbContext _dbContext) : 
        IAuthService
    {
       
        public async Task<ResponseType<string>> RegisterAsync(RegisterRequestDto registerDto)
        {
            var validationErrors = ModelValidation.ModelValidationResponse(registerDto);
            if (validationErrors.Any())
            {
                _logger.LogWarning("REG_001: Validation Failed for registration of {Email}. Invalid fields: {@validationErrors}",
                    registerDto.Email, validationErrors);
                return ResponseType<string>.Fail(
                    validationErrors,
                    "Validation failed. Please check the provided data."
                );
            }
        
            // 1. Checking if User email is already Exist or created
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                _logger.LogError("REG_002: Registration attempt for existing email: {Email}", registerDto.Email);
                return ResponseType<string>.Fail(
                    "EmailAlreadyExists",
                    "Registration failed. An account with this email already exists."
                );
            }
        
            // 2. creating domain users
            var domainUser = new TaskUsers(); // This should likely be persisted in your _dbContext as well
        
            // 3. Create User By new ApplicationUser Object
            var user = new ApplicationUsers
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                DomainUser = domainUser // Ensure DomainUser is configured to be saved with ApplicationUser or separately
            };
        
            // 4. Create the user using UserManager (this handles password hashing and validations)
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogWarning("REG_003: Error occurred when creating user through UserManager for {Email}. Errors: {@Errors}",
                    registerDto.Email, errors);
                return ResponseType<string>.Fail(
                    errors,
                    "Registration failed. Could not create user account."
                );
            }
        
            // 5. Determine role based on config, not hardcoded
            var isAdminEmail = _identitySettings.Value.AdminEmails
                .Any(x => x.Equals(registerDto.Email, StringComparison.CurrentCultureIgnoreCase));

            // If role is equal to isAdminEmail then it's admin, if not, then it's User Role
            var role = isAdminEmail ? Role.Admin : Role.User;  // Use "User" (PascalCase)
        
            // 6. adding role to user based on email
            // Consider handling the result of AddToRoleAsync as well
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(x => x.Description).ToList();
                _logger.LogError("REG_004: Failed to assign role '{Role}' to user {Email}. Errors: {@Errors}",
                    role, registerDto.Email, errors);
                // Decide if this should be a critical failure or if the user can still proceed as a default user
                // For now, we'll treat it as a failure for registration
                return ResponseType<string>.Fail(
                    errors,
                    "Registration successful, but failed to assign user role."
                );
            }
        
            // Save changes to DomainUser if it's managed separately by DbContext
            // If DomainUser is part of ApplicationUsers's aggregate, it might be saved by UserManager.CreateAsync
            // If TaskUsers requires a separate _dbContext.Add(domainUser) and _dbContext.SaveChangesAsync(), add it here.
            // For example:
            // await _dbContext.TaskUsers.AddAsync(domainUser);
            // await _dbContext.SaveChangesAsync();
        
        
            _logger.LogInformation("REG_005: User {Email} successfully registered with role '{Role}'.", user.Email, role);
            return ResponseType<string>.SuccessResult(
                user.Id.ToString(), // Return the user ID as data
                "Registration successful! Your account has been created."
            );
        }
        
        /// <summary>
        /// Login Account Services
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>A DTO containing token and user info if Login succeeds.</returns>
        public async Task<ResponseType<AuthResultDto>> LoginAsync(LoginRequestDto loginDto)
        {
            // 0. Check if information by user is correct
            var validateUser = ModelValidation.ModelValidationResponse(loginDto);
        
            if (validateUser.Any()) // If any error will catch
            {
                _logger.LogWarning("LOG_001: Validation Failed for login of {Email}. Invalid fields: {@validationErrors}",
                    loginDto.Email, validateUser);
                return ResponseType<AuthResultDto>.Fail(
                    validateUser,
                    "Validation failed. Please check your login credentials."
                );
            }
        
            // 1. finding email if it's exist
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
            if (user is null)
            {
                _logger.LogError("LOG_002: Login attempt for non-existent email: {Email}", loginDto.Email);
                return ResponseType<AuthResultDto>.Fail(
                    "InvalidCredentials",
                    "Login failed. Invalid email or password."
                );
            }
        
            // 2. check if password was correct by built-in method
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        
            if (!passwordValid)
            {
                _logger.LogWarning("LOG_003: Invalid password attempt for user {UserId} (Email: {Email})",
                    user.Id, loginDto.Email);
                return ResponseType<AuthResultDto>.Fail(
                    "InvalidCredentials",
                    "Login failed. Invalid email or password."
                );
            }
        
            // 3. Get the roles assigned to the user (could be one or more)
            var roles = await _userManager.GetRolesAsync(user);
        
            // For this simple example, we'll just use the first role (if there's more than one)
            var userRole = roles.FirstOrDefault() ?? "User"; // Default to "User" if no roles found
        
            // 4. generate token a JWT token for user
            var token = await _tokenService.GenerateTokenAsync(new TokenUserDto(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                roles.ToList() // Pass all roles for token generation
            ));
        
            // 5. Create a expiration date (JWT validity is usually handled by the token service)
            DateTime expireAt = DateTime.UtcNow.AddHours(1); // Assuming your token is valid for 1 year
        
            _logger.LogInformation("LOG_004: User {UserId} (Email: {Email}) logged in successfully with role: {UserRole}", user.Id, user.Email, userRole);
            return ResponseType<AuthResultDto>.SuccessResult(
                new AuthResultDto
                {
                    BearerToken = token.BearerToken,
                    ExpiresAt = expireAt,
                    RefreshToken = token.RefreshToken,
                    UserName = user.UserName ?? string.Empty,
                    Role = userRole
                },
                "Login successful! Welcome back."
            );
        }
        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";    
        }   
        
    }
}
