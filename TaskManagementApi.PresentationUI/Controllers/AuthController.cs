using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.Commands.Login;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public AuthController(IMediator mediator,ILogger logger)
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
                _logger.LogInformation("Register Failed, Ruquest is Null");
                return BadRequest(result);
            }
            return Ok(result);
                
            
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
        
    }
}
