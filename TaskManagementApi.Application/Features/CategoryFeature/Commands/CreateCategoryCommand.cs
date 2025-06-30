using MediatR;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record CreateCategoryCommand(CategoryRequestDto dto) :IRequest<ResponseType<CategoryResponseDto>>;

    public class CreateCategoryCommandHandler(ICreateCategoryService create) : IRequestHandler<CreateCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public Task<ResponseType<CategoryResponseDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return create.CreateCategoryAsync(request.dto);
        }
    }
}
