using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record DeleteCategoryCommand(Guid Id) :IRequest<ResponseType<CategoryResponseDto>>;
    public class DeleteCategoryCommandHandler(ICategoryRepository dbContext,
        IGetDomainIdCategoryRepository identityService,
        ILogger<CreateCategoryCommandHandler> logger) : IRequestHandler<DeleteCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public async Task<ResponseType<CategoryResponseDto>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            //get jwt user Id
            var userDomainResponse = await identityService.GetCurrentUserDomainIdDeleteCategoryAsync(request.Id);
            if (!userDomainResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", userDomainResponse.Message);
                return ResponseType<CategoryResponseDto>.Fail(userDomainResponse.Message);
            }
            var categoryToDelete = userDomainResponse.Data;
            
            try
            {
                await dbContext.DeleteAsync(categoryToDelete);

                logger.LogInformation("Successfully deleted category {categoryId}", categoryToDelete);
                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(categoryToDelete),
                    "Category deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category {categoryId}", categoryToDelete);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message,
                    "Failed to delete category");
            }
        }
    }
}
