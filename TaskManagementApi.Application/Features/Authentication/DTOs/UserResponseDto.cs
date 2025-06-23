namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    /// <summary>
    /// Use for HTTP GET
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="UserName"></param>
    /// <param name="Email"></param>
    /// <param name="CreatedAt"></param>
    /// <param name="UpdatedAt"></param>
    public record UserResponseDto(
    
        Guid Id,
        string UserName,
        string Email,

        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}
