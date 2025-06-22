using Microsoft.AspNetCore.Identity;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Application.Interfaces;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
   /* public class LoginCommand : IAccountService
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        public LoginCommand(ITokenService tokenService,UserManager<ApplicationUsers> userManager,RoleManager<ApplicationRole> roleManager)
        {                                              
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public Task<ResponseType<UserResponseDto>> LoginAsync(UserLoginRequestDto loginDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a new user with a given role.
        /// </summary>
        /// <param name="dto">The user's registration data.</param>
        /// <param name="role">The role to assign to the user (e.g. "User").</param>
        /// <returns>A DTO containing token and user info if registration succeeds.</returns>
        public async Task<ResponseType<UserResponseDto>> RegisterAsync(UserRegisterRequestDto registerDto, string role)
        {
            ResponseType<UserResponseDto> response = new();

            //1. Checking if User email is already Exist or created
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if(existingUser != null)
            {
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
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // 3. Create the user using UserManager (this handles password hashing and validations
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                //Collect all errors and throw it
                var errors = string.Join(",", result.Errors.Select(x => x.Description));
                response.Success = false;
                response.Errors?.Add(errors);
            }
            response.Success = true;

        }
    }*/
}
