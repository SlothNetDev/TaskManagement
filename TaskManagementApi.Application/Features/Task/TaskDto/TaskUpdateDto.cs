
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementApi.Domains.Enums;

namespace TaskManagementApi.Application.Features.Task.TaskDto
{
    public class TaskUpdateDto
    {
        [Required(ErrorMessage = "Id is Required")]
        public Guid Id { get; set; }
    
        [Required(ErrorMessage = "Title is Required")]
        [StringLength(120, ErrorMessage = "Title Cannot exceed 120 characters")]
        public string? Title { get; set; }
    
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority? Priority { get; set; }
    
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? Status { get; set; }

    }

}
