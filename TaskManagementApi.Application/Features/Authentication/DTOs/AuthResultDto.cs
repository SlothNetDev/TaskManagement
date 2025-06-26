using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    public record AuthResultDto(
        string Token,
        DateTime ExpiresAt,
        string RefreshToken,
        string UserName,
        string Role
        );
}
