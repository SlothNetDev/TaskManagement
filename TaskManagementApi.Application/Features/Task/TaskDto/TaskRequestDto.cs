using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.DTOs.TaskDto
{
    public class TaskRequestDto
    {
        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Title is Required")]
        [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose Priority")]
        public Priority Priority { get; set; }

        [Required(ErrorMessage = "Decide a Due Date")]
        public DateTime DueDate { get; set; }

        public TaskRequestDto(TaskItem item)
        {
            CategoryId = item.CategoryId;
            Title = item.Title;
            Priority = item.Priority;
            DueDate = item.DueDate ?? DateTime.UtcNow.AddDays(1); // make due date as 1 day if not given
        }
    }
}
