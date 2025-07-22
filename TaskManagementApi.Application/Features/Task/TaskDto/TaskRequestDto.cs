using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.Features.Task.TaskDto
{
    public record TaskRequestDto
    {
        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; init; }

        [Required(ErrorMessage = "Title is Required")]
        [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
        public string Title { get; init; } = string.Empty;

        [Required(ErrorMessage = "Choose Priority")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority Priority { get; init; }

        [Required(ErrorMessage = "Decide a Due Date")]
        public DateTime DueDate { get; init; }
    }
}
