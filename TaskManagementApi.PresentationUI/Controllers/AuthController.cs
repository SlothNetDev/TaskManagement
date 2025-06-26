using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.DTOs.Authentication;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));
        
            if (!result.Success)
            {
                _logger.LogWarning(" Register failed for {Email}", request.Email);
                return BadRequest(new { errors = result.Errors });
            }
        
            _logger.LogInformation("User registered Successfully");
            return Ok(request);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
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
        [HttpPost("logOut")]
        public async Task<IActionResult> LogOut([FromBody] LogOutCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
            {
                _logger.LogWarning("Cannot Logout Account Id: {ID}", result.Data);
                return BadRequest(result);
            }
            return Ok(result);
        }
        
    }
}
