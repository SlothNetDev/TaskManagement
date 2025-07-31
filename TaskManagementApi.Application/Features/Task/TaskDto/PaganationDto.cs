using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Task.TaskDto
{
    public record PaganationDto
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }
}
