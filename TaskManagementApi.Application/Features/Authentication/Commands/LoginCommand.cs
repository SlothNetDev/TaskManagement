
using MediatR;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs.Authentication;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    /// <summary>
    /// Represents a command to initiate a login operation using the provided user credentials.
    /// </summary>
    /// <param name="Dto">The data transfer object containing the user's login credentials, such as username and password. This parameter
    /// cannot be null.</param>
    public record LoginCommand(LoginRequestDto Dto) : IRequest<ResponseType<AuthResultDto>>;

    //Processing Request
    public class LoginCommandHandler(IAuthService _identity) : IRequestHandler<LoginCommand, ResponseType<AuthResultDto>>
    {
        public async Task<ResponseType<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _identity.LoginAsync(request.Dto);
        }
    }
}
