using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TaskManagementApi.Application.ApplicationHelpers.CostumeValidation;

namespace TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto
{
    public record CategoryUpdateDto
    {
        [NotDefaultGuid(ErrorMessage = "Id cannot be empty")]
        [Required(ErrorMessage = "Id is Required")]
        public Guid Id { get; init; }
    
        [StringLength(120, ErrorMessage = "Category Name Cannot exceed with 120 characters")]
        public string? CategoryName { get; init; }
    
        [StringLength(1000, ErrorMessage = "Descriptions Cannot exceed with 1000 characters")]
        public string? Description { get; init; }
    }
}
