using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.Application.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly IIdentityService _identity;
        public LoginCommandHandler(IIdentityService identity)
        {
            _identity = identity;
        }
        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _identity.LoginAsync(request.Dto);
        }
    }
}
