using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Task.TaskDto
{
    public record PaganationDto(
        [Range(1,int.MaxValue,ErrorMessage = "Page number must be greater than 0.")]
        int PageNumber = 1,
        [Range(1,100,ErrorMessage = "Page size must be between 1 and 100.")]
        int PageSize = 10
        );
}
