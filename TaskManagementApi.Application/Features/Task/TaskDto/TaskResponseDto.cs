using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Application.Features.Task.TaskDto
{
    public record TaskResponseDto(
          Guid Id,
          string Title,
          string Priority, 
          string Status,   
          DateTime? DueDate,
          DateTime? CreatedAt,
          DateTime? UpdatedAt
    )
    {
        public TaskResponseDto(TaskItem task): this(
            task.Id,
            task.Title,
            task.Priority.ToString() ?? string.Empty, //make this string empty as default
            task.Status.ToString() ?? string.Empty,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt
            )
        { }
    };
}
