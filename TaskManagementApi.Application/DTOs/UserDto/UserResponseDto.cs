
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Domains.Entities;
namespace TaskManagementApi.Application.DTOs.UserDto
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
    
        string UserName,
        string Email,

        DateTime? CreatedAt,
        DateTime? UpdatedAt
    )
    {
        public UserResponseDto(ApplicationUsers users) : this(
            users.UserName ?? string.Empty,
            users.Email ?? string.Empty,
            users.CreatedAt,
            users.UpdatedAt)
        { }
    };
}
