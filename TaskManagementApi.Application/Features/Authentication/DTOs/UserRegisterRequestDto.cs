using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    public record UserRegisterRequestDto(
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
