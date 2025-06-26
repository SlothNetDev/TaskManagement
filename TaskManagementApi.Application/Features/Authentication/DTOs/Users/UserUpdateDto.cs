using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Application.Features.Authentication.DTOs.Users
{
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
}
