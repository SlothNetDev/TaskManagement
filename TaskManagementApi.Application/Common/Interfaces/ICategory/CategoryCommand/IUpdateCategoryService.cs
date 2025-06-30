using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand
{
    public interface IUpdateCategoryService
    {
        Task<ResponseType<CategoryResponseDto>> UpdateCategoriesAsync(CategoryUpdateDto update);
    }
}
