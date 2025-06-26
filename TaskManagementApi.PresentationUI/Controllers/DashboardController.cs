using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.PresentationUI.Controllers
{
    
    [ApiController]
    [Route("auth")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(IMediator mediator,ILogger<DashboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [AllowAnonymous]
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
            return Ok(result);
        }

        [AllowAnonymous]
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

        [Authorize]
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

        [Authorize]
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
