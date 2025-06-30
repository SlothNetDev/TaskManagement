using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record UpdateCategoryCommand(CategoryUpdateDto dto): IRequest<ResponseType<CategoryResponseDto>>;

    public class UpdateCategoryCommandHandler(IUpdateCategoryService service) : IRequestHandler<UpdateCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public async Task<ResponseType<CategoryResponseDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await service.UpdateCategoriesAsync(request.dto);
        }
    }
}
