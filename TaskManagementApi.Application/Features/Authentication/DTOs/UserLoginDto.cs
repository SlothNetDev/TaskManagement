using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
     public record UserLoginRequestDto(
        [Required(ErrorMessage = "UserName is Required")] // Or use Email if you allow login by email
        string UserName,

        [Required(ErrorMessage = "Password is Required")]
        string Password
    );
}
