using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Application.Features.CategoryFeature.Commands;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("category")]
    [ApiController]
    [Authorize]
    public class CategoryController(ILogger<TaskItemController> _logger,
        Mediator _mediaR) : ControllerBase
    {
        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequestDto request)
        {
            var result = await _mediaR.Send(new CreateCategoryCommand(request));
            if (!result.Success)
            {
                _logger.LogWarning("Creating {categoryName} Failed", result.Data.CategoryName);
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
