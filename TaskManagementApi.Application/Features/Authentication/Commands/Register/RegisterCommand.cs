
using MediatR;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public record RegisterCommand : IRequest<AuthResultDto>;
}
