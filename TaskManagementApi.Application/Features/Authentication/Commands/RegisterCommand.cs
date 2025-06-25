
using MediatR;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    //Sending request 
    public record RegisterCommand(UserRegisterRequestDto dto) : IRequest<ResponseType<AuthResultDto>>;

    //Handling Request
    public class RegisterCommandHandler(IIdentityService<AuthResultDto> _identity) : IRequestHandler<RegisterCommand, ResponseType<AuthResultDto>>
    {
        public async Task<ResponseType<AuthResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _identity.RegisterAsync(request.dto);
        }
    }

}
