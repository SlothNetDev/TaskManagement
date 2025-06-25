using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.Authentication.Commands
{
    public class RefreshTokenCommand : IRequest<ResponseType<AuthResultDto>>
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenCommandHandler(ITokenService _refreshToken) : IRequestHandler<RefreshTokenCommand, ResponseType<AuthResultDto>>
    {
        public async Task<ResponseType<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _refreshToken.RefreshTokenAsync(request.Token,request.RefreshToken);
        }
    }

}
