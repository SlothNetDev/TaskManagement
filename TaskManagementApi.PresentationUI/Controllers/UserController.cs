using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.User.Queries;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("account")]
    [ApiController]
    public class UserController(IMediator _mediaR,ILogger<UserController> _logger) : ControllerBase
    {
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediaR.Send(new GetProfileQuery());

            if (!result.Success)
            {
                _logger.LogWarning(" Register Your Email {Email} First", result.Data?.Email);
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
