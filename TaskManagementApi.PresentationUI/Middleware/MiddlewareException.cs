using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace TaskManagementApi.PresentationUI.Middleware
{
    /// <summary>
    /// Cateches Unhandle exceptions globally and logs them
    /// With C# 12 Primary Constructor, explicitly storing in fields
    /// </summary>
    public class MiddlewareException(RequestDelegate _next,ILogger<ExceptionHandlerMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass through to the next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the full exception
                _logger.LogError(ex, "🚨 Unhandled exception caught in middleware");

                // Return a 500 response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    status = context.Response.StatusCode,
                    message = "An unexpected error occurred.",
                    detail = ex.Message // ⚠️ include more detail only in dev
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
