using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    public class UserDto
    {

        /// <summary>
        /// Profile of User Dto
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserName"></param>
        /// <param name="Email"></param>
        /// <param name="Roles"></param>
        public record UserProfileDto(
        string Id,
        string UserName,
        string Email,
        List<string> Roles
        );

        /// <summary>
        /// Used for HTTPatch 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Email"></param>
        public record UserUpdateDto(

            [Required(ErrorMessage = "Id is Required")]
            Guid Id,

            [RegularExpression("^[a-zA-Z0-9_]{3,20}$")]
            string? UserName,

            [RegularExpression("^[\\w\\.-]+@[\\w\\.-]+\\.\\w{2,}$")]
            string? Email
        );


        /// <summary>
        /// Logging out Request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="RefreshToken"></param>
        public record LogOutRequestDto(
            string Token,
            string RefreshToken
            );
        }
}
