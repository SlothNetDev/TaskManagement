using MediatR;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseType<AuthResultDto>>
    {
        private readonly IIdentityService _identity;
        public RegisterCommandHandler(IIdentityService identity)
        {
            _identity = identity;
        }
        public async Task<ResponseType<AuthResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _identity.RegisterAsync(request.dto);
        }
    }
}
