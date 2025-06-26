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
    public record RefreshTokenCommand(string Token,string RefreshToken) : IRequest<ResponseType<RefreshTokenResponseDto>>;

    public class RefreshTokenCommandHandler(IRefreshTokenService _identityService) : IRequestHandler<RefreshTokenCommand, ResponseType<RefreshTokenResponseDto>>
    {
        public async Task<ResponseType<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RefreshTokenAsync(request.Token,request.RefreshToken);
        }
    }

}
