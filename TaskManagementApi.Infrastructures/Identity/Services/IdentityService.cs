using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;
using TaskManagementApi.Application.DTOs;
using Microsoft.Extensions.Options;
using TaskManagementApi.Domains.Entities;
using TaskManagement.Infrastructures.Data;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public class IdentityService(IOptions<IdentitySettings> _identitySettings,ILogger<IdentityService> _logger,
        UserManager<ApplicationUsers> _userManager,ITokenService _tokenService,TaskManagementDbContext _dbContext) : IIdentityService
    {
       
        

        /// <summary>
        /// Registers a new user with a given role.
        /// </summary>
        /// <param name="dto">The user's registration data.</param>
        /// <param name="role">The role to assign to the user (e.g. "User").</param>
        /// <returns>A DTO containing token and user info if registration succeeds.</returns>
        public async Task<ResponseType<AuthResultDto>> RegisterAsync(UserRegisterRequestDto registerDto)
        {
            ResponseType<AuthResultDto> response = new();

            var validationErrors = ModelValidation.ModelValidationResponse(registerDto);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Validation Failed for {Email}. Invalid fields: {@validationError}",
                     registerDto.Email,validationErrors);
                response.Success = false;
                response.Message = "There are Model Validation Occurred";
                return response;

            }

            //1. Checking if User email is already Exist or created
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if(existingUser is not null)
            {
                _logger.LogError("Login attempt for Existing email: {Email}", registerDto.Email);
                response.Success = false;
                response.Message = "Invalid Email";
                response.Errors?.Add("Email is Already Exist");
                return response;
            }

            //2. creating domain users 
            var domainUser = new Users();


            //3. Create User By new ApplicationUser Object
            var user = new ApplicationUsers
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                DomainUser = domainUser
            };

            // 4. Create the user using UserManager (this handles password hashing and validations
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                //Collect all errors and throw it
                _logger.LogWarning("Error Occured When Creating user Manager, Reason: {@Error}", result.Errors);
                var errors = string.Join(",", result.Errors.Select(x => x.Description));
                response.Success = false;
                response.Errors?.Add(errors);
            }

            // 5. Determine role based on config, not hardcoded
            var isAdminEmail = _identitySettings.Value.AdminEmails
                .Any(x => x.Equals(registerDto.Email, StringComparison.CurrentCultureIgnoreCase));

            //if role is equal to isAdminEmail then it's admin, if not, then it's User Role
            var role = isAdminEmail ? "Admin" : "User";

            //6. adding role to user based on email
            await _userManager.AddToRoleAsync(user, role);


            //7. Generate JWT token 
            var token = await _tokenService.GenerateTokenAsync(new TokenUserDto(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                new List<string>{ "User"}));

            //8.Expired Time
            DateTime expireAt = DateTime.UtcNow.AddDays(7);

            try
            {
                response.Success = true;
                response.Message = "Successfully Register your Account";
                response.Data = new AuthResultDto(token,expireAt,user.UserName ?? string.Empty,role);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Register failed for account. Reason: {ErrorMessage}", ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
        /// <summary>
        /// Login Account Services
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>A DTO containing token and user info if Login succeeds.</returns>
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

            if(user is null)
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
            var token = await _tokenService.GenerateTokenAsync(new TokenUserDto(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                new List<string>{ "User"}));

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
