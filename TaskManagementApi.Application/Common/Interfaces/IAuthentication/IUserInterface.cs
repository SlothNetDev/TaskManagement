using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Authentication.DTOs.Users;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IAuthentication
{
    public interface IUserInterface
    {
        Task<ResponseType<UserProfileDto>> UserProfileAsync();
    }
}
