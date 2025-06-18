using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Application.DTOs.CategoriesDto
{
    public record CategoryRequestDto(
        [Required(ErrorMessage = "Category Name is Required")]
        [StringLength(120, ErrorMessage = "Category Name Cannot exceed with 120 characters")]
        string CategoryName,
        [StringLength(1000, ErrorMessage = "Descriptions Cannot exceed with 1000 characters")]
        string? Description
        )
    {
        public CategoryRequestDto(Category category):this(
            category.CategoryName,
            category.Description)
        { }
    }
}
