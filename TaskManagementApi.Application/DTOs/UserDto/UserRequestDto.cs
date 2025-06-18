using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.DTOs.UserDto
{
    /// <summary>
    /// For HTTP Post
    /// </summary>
    /// <param name="UserName"></param>
    /// <param name="Email"></param>
    /// <param name="Password"></param>
    public record UserRequestDto(
        [Required(ErrorMessage = "UserName is Required")]
        [RegularExpression("^[a-zA-Z0-9_]{3,20}$")]
        string UserName,

        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression("^[\\w\\.-]+@[\\w\\.-]+\\.\\w{2,}$")]
        string Email,

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$")]
        string Password
    );
}
