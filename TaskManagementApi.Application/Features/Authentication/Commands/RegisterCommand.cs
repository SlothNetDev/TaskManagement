using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Application.Interfaces;
using TaskManagementApi.Application.Interfaces.IAuthentication;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public class RegisterCommand : IRegisterCommand
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        public RegisterCommand(ITokenService tokenService,UserManager<ApplicationUsers> userManager,RoleManager<ApplicationRole> roleManager)
        {                                              
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        

        /// <summary>
        /// Registers a new user with a given role.
        /// </summary>
        /// <param name="dto">The user's registration data.</param>
        /// <param name="role">The role to assign to the user (e.g. "User").</param>
        /// <returns>A DTO containing token and user info if registration succeeds.</returns>
        public async Task<ResponseType<AuthResultDto>> RegisterAsync(UserRegisterRequestDto registerDto, string role)
        {
            ResponseType<AuthResultDto> response = new();

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
                CreatedAt = DateTime.UtcNow,
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

            // 4. Check if the role exists — create it if not (optional safety)
            if(!await _roleManager.RoleExistsAsync(role))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = "Failed to create role";
                    return response;
                }
            }

            // 5. Assign role to the user
            var roleAssign = await _userManager.AddToRoleAsync(user, role);
            if (!roleAssign.Succeeded)
            {
                 response.Success = false;
                 response.Message = "Failed to assign role to user.";
                 return response;
            }

            //6. Generate JWT token 
            var token = await _tokenService.GenerateTokenAsync(user);

            //7.Expired Time
            DateTime expireAt = DateTime.UtcNow.AddYears(1);

            response.Success = true;
            response.Message = "Successfully Register your Account";
            response.Data = new AuthResultDto(token,expireAt,user.UserName,role);
            return response;
        }
    }
}
