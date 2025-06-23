using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.DTOs
{
    public record TokenUserDto(
        string UserId,
        string UserName,
        string Email,
        List<string> Roles
        );
}
