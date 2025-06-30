using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record DeleteCategoryCommand(Guid Id) :IRequest<ResponseType<CategoryResponseDto>>;
    public class DeleteCategoryCommandHandler(IDeleteCategoryService service) : IRequestHandler<DeleteCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public async Task<ResponseType<CategoryResponseDto>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            return await service.DeleteCategoryAsync(request.Id);
        }
    }
}
