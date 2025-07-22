using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto
{
    public record CategoryResponseDto(
        Guid Id,      
        string CategoryName,   
        string Description)
    {
        public CategoryResponseDto(Category category) : this(
            category.Id,
            category.CategoryName,
            category.Description ?? string.Empty
            )
        { }
    };
    public record CategoryResponseDtoWithTask(
        Guid Id,      
        string CategoryName,   
        string Description,
        List<TaskResponseDto> Tasks);
}
