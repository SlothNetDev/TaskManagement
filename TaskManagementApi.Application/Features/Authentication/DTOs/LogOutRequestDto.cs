using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
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
