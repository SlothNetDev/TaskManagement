using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryQuery;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;
namespace TaskManagementApi.Application.Features.CategoryFeature.Queries
{
    public record GetAllCategoriesQuery() : IRequest<ResponseType<List<CategoryResponseDtoWithTask>>>;
    public class GetAllCategoriesQueryHanlder(IGetAllCategories dto) : IRequestHandler<GetAllCategoriesQuery, ResponseType<List<CategoryResponseDtoWithTask>>>
    {
        public async Task<ResponseType<List<CategoryResponseDtoWithTask>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await dto.GetAllCategoriesAsync();
        }
    }
}
