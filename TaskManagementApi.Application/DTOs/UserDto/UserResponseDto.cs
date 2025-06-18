using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManagementApi.Domain.Entities;

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
    
        Guid Id,
        string UserName,
        string Email,

        DateTime? CreatedAt,
        DateTime? UpdatedAt
    )
    {
        public UserResponseDto(Users users) : this(
            users.Id,
            users.UserName ?? string.Empty,
            users.Email ?? string.Empty,
            users.CreatedAt,
            users.UpdatedAt)
        { }
    };
}
