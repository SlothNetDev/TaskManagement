
using MediatR;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public record RegisterCommand(UserRegisterRequestDto dto) : IRequest<ResponseType<AuthResultDto>>;
}
