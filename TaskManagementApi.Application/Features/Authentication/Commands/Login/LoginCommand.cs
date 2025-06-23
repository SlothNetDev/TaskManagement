
using MediatR;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// Login Account Service
    /// </summary>
    public record LoginCommand(UserLoginRequestDto Dto) : IRequest<AuthResultDto>;

}
