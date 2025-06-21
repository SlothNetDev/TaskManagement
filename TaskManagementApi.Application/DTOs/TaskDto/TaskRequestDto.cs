using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.DTOs.TaskDto
{
    public record TaskRequestDto(
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
    string Title,
    [Required(ErrorMessage = "Choose Priority")]
    Priority Priority
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority Priority { get; init; } = Priority;

    }
}
