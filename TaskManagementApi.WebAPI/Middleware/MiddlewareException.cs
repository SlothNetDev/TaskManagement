using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace TaskManagementApi.WebAPI.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions globally and logs them.
    /// With C# 12 Primary Constructor, explicitly storing in fields.
    /// </summary>
    public class MiddlewareException(RequestDelegate _next, ILogger<ExceptionHandlerMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Unhandled exception caught in middleware");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    status = context.Response.StatusCode,
                    message = "Sorry, something went wrong. Please try again later.",
#if DEBUG
                    detail = ex.Message
#else
                        detail = "Internal server error."
#endif
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
