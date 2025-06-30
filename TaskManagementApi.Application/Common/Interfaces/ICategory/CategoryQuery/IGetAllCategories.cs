using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryQuery
{
    public interface IGetAllCategories
    {
        Task<ResponseType<List<CategoryResponseDtoWithTask>>> GetAllCategoriesAsync();
    }
}
