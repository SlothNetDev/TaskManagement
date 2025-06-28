using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("task")]
    [ApiController]
    [Authorize]
    public class TaskItemController(ILogger<TaskItemController> _logger,
        Mediator _mediaR) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskRequestDto requestDto)
        {
            var result = await _mediaR.Send(new CreateTaskCommand(requestDto));

            if (!result.Success)
            {
                _logger.LogWarning("Creating Task Titled {title}", result.Data.Title);
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskUpdateDto requestDto)
        {
            var result = await _mediaR.Send(new UpdateTaskCommand(requestDto));

            if (!result.Success)
            {
                _logger.LogWarning("Updating Task Titled {title}", result.Data.Title);
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
