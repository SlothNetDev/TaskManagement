using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Features.User.Queries;

namespace TaskManagementApi.WebAPI.Controllers
{
    [Authorize]
    [Route("account")]
    [ApiController]
    public class UserController(IMediator _mediaR,ILogger<UserController> _logger) : ControllerBase
    {
        
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

        [HttpPost("upload-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // You can optionally specify consumes to indicate content type, though often inferred
        // [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSingleImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Basic validation (e.g., file type, size)
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Only image files are allowed.");
            }
            if (file.Length > 5 * 1024 * 1024) // 5 MB limit
            {
                return BadRequest("File size exceeds 5MB.");
            }

            // Generate a unique filename and path
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var newFileName = $"{fileName}_{System.Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newFileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Save the file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { FileName = newFileName, Size = file.Length, ContentType = file.ContentType });
        }
    }
}
