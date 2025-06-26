using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;
using static TaskManagementApi.Application.Features.Authentication.DTOs.UserDto;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    //Processing Request
    public record LogOutCommand(LogOutRequestDto dto) : IRequest<ResponseType<string>>;

    //Handling request
    public class LogoutCommandHandler(
        IRefreshTokenService _refreshTokenService // Use an interface defined in Application
    ) : IRequestHandler<LogOutCommand, ResponseType<string>>
    {
        public async Task<ResponseType<string>> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            // Delegate to the service inside Infrastructure
            return await _refreshTokenService.LogoutAsync(request.dto);
        }
    }

}
