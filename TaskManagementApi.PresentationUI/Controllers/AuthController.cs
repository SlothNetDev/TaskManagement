using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.PresentationUI.Controllers
{
    
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IMediator mediator,ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));
        
            if (!result.Success)
            {
                _logger.LogWarning(" Register failed for {Email}", request.Email);
                return BadRequest(new { errors = result.Errors });
            }
        
            _logger.LogInformation(" User registered: {User}", result.Data?.UserName);
            return Ok(new AuthResultDto(
                result.Data.Token,
                result.Data.ExpiresAt,
                result.Data.RefreshToken,
                result.Data.UserName,
                result.Data.Role
            ));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
        {
            var result = await _mediator.Send(new LoginCommand(request));
            if (!result.Success)
            {
                _logger.LogInformation("Login Failed, Ruquest is Null");
                return BadRequest(result);
            }
            return Ok(result);
                
            
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {

            var result = await _mediator.Send(command);
            if (!result.Success)
            {
                _logger.LogWarning("Something wrong from Refresh Token");
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
