using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domains.Entities;

namespace TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto
{
    public record CategoryRequestDto
    {
        [Required(ErrorMessage = "Category Name is Required")]
        [StringLength(120, ErrorMessage = "Category Name Cannot exceed with 120 characters")]
        public string CategoryName { get; init; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descriptions Cannot exceed with 1000 characters")]
        public string? Description { get; init; }
    }
}
