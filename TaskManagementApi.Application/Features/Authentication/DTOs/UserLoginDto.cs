using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
     public record UserLoginRequestDto(
        [Required(ErrorMessage = "Email is Required")] // Or use Email if you allow login by email
        string Email,

        [Required(ErrorMessage = "Password is Required")]
        string Password
    );
}
