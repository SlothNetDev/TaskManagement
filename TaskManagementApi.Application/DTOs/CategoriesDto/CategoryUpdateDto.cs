using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Application.DTOs.CategoriesDto
{
    public record CategoryUpdateDto(
        [Required(ErrorMessage = "Id is Required")]
        Guid Id,

        [StringLength(120, ErrorMessage = "Category Name Cannot exceed with 120 characters")]
        string? CategoryName,

        [StringLength(1000, ErrorMessage = "Descriptions Cannot exceed with 1000 characters")]
        string? Description
        )
    {
        public CategoryUpdateDto(Category category): this(
            category.Id,
            category.CategoryName,
            category.Description)
        { }
    }
}
