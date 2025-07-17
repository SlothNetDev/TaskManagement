using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Application.Features.Task.Query;
using TaskManagementApi.Application.Features.Task.TaskDto;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("task")]
    [ApiController]
    [Authorize]
    public class TaskItemController(ILogger<TaskItemController> _logger,
        IMediator _mediaR) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskRequestDto requestDto)
        {
            var result = await _mediaR.Send(new CreateTaskCommand(requestDto));

            if (!result.Success)
            {
                _logger.LogWarning("Creating Task Titled {title} Failed", result.Data.Title ?? null);
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPatch("update")]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskUpdateDto requestDto)
        {
            var result = await _mediaR.Send(new UpdateTaskCommand(requestDto));

            if (!result.Success)
            {
                _logger.LogWarning("Updating Task Titled {title} Failed", result.Data.Title ?? null);
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTask(string search)
        {
            var result = await _mediaR.Send(new SearchTaskQuery(search));
            if (!result.Success)
            {
                _logger.LogWarning("Searching Task {item} Failed", search);
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllTask")]
        public async Task<IActionResult> GetAllTask()
        {
            var result = await _mediaR.Send(new GetAllTaskQuery());
            if (!result.Success)
            {
                _logger.LogWarning("Get All Task Failed, Reason: {reason}",result.Message);
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetTaskById{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await _mediaR.Send(new GetByIdTaskQuery(id));
            if (!result.Success)
            {
                _logger.LogWarning("Get Task Failed, Reason: {reason}",result.Message);
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpGet("paganation")]
        public async Task<IActionResult> GetPaganatedAsync(PaganationDto request,CancellationToken token)
        {
            var result = await _mediaR.Send(new GetPaganationTasksQuery(request, token));

            if (!result.Success)
            {
                _logger.LogInformation("Get All Task Failed, Reason: {reason}", result.Message);
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTaskAsync(Guid id)
        {
            var result = await _mediaR.Send(new DeleteTaskCommand(id));

            if (!result.Success)
            {
                _logger.LogWarning("Deleting Task Titled {title} Failed", result.Data.Title ?? null);
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
