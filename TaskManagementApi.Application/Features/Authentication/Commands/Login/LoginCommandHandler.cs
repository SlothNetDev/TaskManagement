using MediatR;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// MediaR handler
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseType<AuthResultDto>>
    {
        private readonly IIdentityService _identity;
        public LoginCommandHandler(IIdentityService identity)
        {
            _identity = identity;
        }
        public async Task<ResponseType<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _identity.LoginAsync(request.Dto);
        }
    }
}
