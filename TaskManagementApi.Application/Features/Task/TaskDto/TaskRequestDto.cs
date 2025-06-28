using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.DTOs.TaskDto
{
    public class TaskRequestDto
    {
        [Required(ErrorMessage = "Title is Required")]
        [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose Priority")]
        public Priority Priority { get; set; }

        [Required(ErrorMessage = "Decide a Due Date")]
        public DateTime DueDate { get; set; }
    }
}
