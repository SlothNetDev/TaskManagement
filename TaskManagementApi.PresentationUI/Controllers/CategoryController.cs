using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Application.Features.CategoryFeature.Commands;
using TaskManagementApi.Application.Features.CategoryFeature.Queries;

namespace TaskManagementApi.PresentationUI.Controllers
{
    [Route("category")]
    [ApiController]
    [Authorize]
    public class CategoryController(ILogger<CategoryController> _logger,
        IMediator _mediaR) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequestDto request)
        {
            var result = await _mediaR.Send(new CreateCategoryCommand(request));
            if (!result.Success)
            {
                _logger.LogWarning("Creating {categoryName} Failed", result.Data?.CategoryName);
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategoryAsync()
        {
            var result = await _mediaR.Send(new GetAllCategoriesQuery());
            if (!result.Success)
            {
                _logger.LogWarning("Successfully Retrieve all Categories");
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("getByIdCategories{id}")]
        public async Task<IActionResult> GetByIdCategoryAsync(Guid id)
        {
            var result = await _mediaR.Send(new GetByIdCategoriesQuery(id));
            if (!result.Success)
            {
                _logger.LogWarning("Successfully Retrieve Categories");
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] CategoryUpdateDto request)
        {
            var result = await _mediaR.Send(new UpdateCategoryCommand(request));
            if (!result.Success)
            {
                _logger.LogWarning("Updating {categoryName} Failed", result.Data?.CategoryName);
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("delete{Id}")]
        public async Task<IActionResult> DeleteCategoriesAsync([FromBody] Guid Id)
        {
            var result = await _mediaR.Send(new DeleteCategoryCommand(Id));
            if (!result.Success)
            {
                _logger.LogWarning("Deleting CategoryName {name} Failed", result.Data?.CategoryName);
                return NotFound(result);
            }
            return Ok(result);
        }
    }
    
}
