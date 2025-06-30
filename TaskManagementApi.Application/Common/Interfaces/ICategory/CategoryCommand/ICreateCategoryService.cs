using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand
{
    public interface ICreateCategoryService
    {
        Task<ResponseType<CategoryResponseDto>> CreateCategoryAsync(CategoryRequestDto requestDto);
    }
}
