using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs.Users
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
       

}
