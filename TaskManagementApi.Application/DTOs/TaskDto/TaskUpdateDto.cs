
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.DTOs.TaskDto
{
    public record TaskUpdateDto(
        [Required(ErrorMessage = "Id is Required")]
        Guid Id,

        [Required(ErrorMessage = "Title is Required")]
        [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
        string? Title,
        Priority? Priority,
        Status? Status)
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority? Priority { get; } = Priority;
    
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? Status { get; init; } = Status;
        
    };
}
