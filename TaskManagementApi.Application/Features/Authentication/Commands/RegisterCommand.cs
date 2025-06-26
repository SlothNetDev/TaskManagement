
using MediatR;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs.Authentication;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    //Sending request 
    public record RegisterCommand(RegisterRequestDto dto) : IRequest<ResponseType<string>>;

    //Handling Request
    public class RegisterCommandHandler(IAuthService _identity) : IRequestHandler<RegisterCommand, ResponseType<string>>
    {
        public async Task<ResponseType<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _identity.RegisterAsync(request.dto);
        }
    }

}
