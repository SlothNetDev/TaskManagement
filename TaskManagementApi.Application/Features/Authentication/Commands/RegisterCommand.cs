using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public class RegisterCommand : IRegisterCommand
    {
        private readonly IdentitySettings _identitySettings;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        public RegisterCommand(ITokenService tokenService,UserManager<ApplicationUsers> userManager,RoleManager<ApplicationRole> roleManager,ILogger logger,IdentitySettings identitySettings)
        {                                              
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _identitySettings = identitySettings;
        }
        

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
            if(existingUser != null)
            {
                _logger.LogError("Login attempt for Existing email: {Email}", registerDto.Email);
                response.Success = false;
                response.Message = "Invalid Email";
                response.Errors?.Add("Email is Already Exist");
                return response;
            }

            //2. Create User By new ApplicationUser Object
            var user = new ApplicationUsers
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // 3. Create the user using UserManager (this handles password hashing and validations
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                //Collect all errors and throw it
                _logger.LogWarning("Error Occured When Creating user Manager, Reason: {@Error}", result.Errors);
                var errors = string.Join(",", result.Errors.Select(x => x.Description));
                response.Success = false;
                response.Errors?.Add(errors);
            }

            // 4. Determine role based on config, not hardcoded
            var isAdminEmail = _identitySettings.AdminEmails
                .Any(x => x.Equals(registerDto.Email, StringComparison.CurrentCultureIgnoreCase));

            //if role is equal to isAdminEmail then it's admin, if not, then it's User Role
            var role = isAdminEmail ? "Admin" : "User";

            //5. adding role to user based on email
            await _userManager.AddToRoleAsync(user, role);


            //6. Generate JWT token 
            var token = await _tokenService.GenerateTokenAsync(user);

            //7.Expired Time
            DateTime expireAt = DateTime.UtcNow.AddDays(7);

            try
            {
                response.Success = true;
                response.Message = "Successfully Register your Account";
                response.Data = new AuthResultDto(token,expireAt,user.UserName,role);
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
    }
}
