
using MediatR;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public record RegisterCommand(UserRegisterRequestDto dto) : IRequest<AuthResultDto>;
}
