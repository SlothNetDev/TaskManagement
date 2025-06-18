using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.DTOs.CategoriesDto
{
    public record CategoryResponseDto(
        Guid Id,      
        string CategoryName,   
        string Description)
    {
        public CategoryResponseDto(Category category):this(
            category.Id,
            category.CategoryName,
            category.Description ?? string.Empty)
        { }
    }
}
