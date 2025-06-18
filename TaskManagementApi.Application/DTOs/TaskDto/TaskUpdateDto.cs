using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Domain.Enums;

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
        
        public TaskUpdateDto(Tasks tasks):this(
            tasks.Id,
            tasks.Title,
            tasks.Priority,
            tasks.Status)
        { }
    };
}
