using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.IUser;
using TaskManagementApi.Application.Features.Users.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.User.Queries
{
    public record GetProfileQuery() : IRequest<ResponseType<UserProfileDto>>;
    public class GetProfileQueryHandler(IUserService _userService) : IRequestHandler<GetProfileQuery, ResponseType<UserProfileDto>>
    {
        public async Task<ResponseType<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            return await _userService.UserProfileAsync();
        }
    }
    
}
