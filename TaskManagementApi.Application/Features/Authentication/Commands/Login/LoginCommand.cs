
using MediatR;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// Represents a command to initiate a login operation using the provided user credentials.
    /// </summary>
    /// <param name="Dto">The data transfer object containing the user's login credentials, such as username and password. This parameter
    /// cannot be null.</param>
    public record LoginCommand(UserLoginRequestDto Dto) : IRequest<ResponseType<AuthResultDto>>;

}
