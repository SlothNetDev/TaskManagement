using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Application.Features.CategoryFeature.Commands;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Queries;

public record GetByIdCategoriesQuery(Guid id) : IRequest<ResponseType<CategoryResponseDto>>;

public  class GetByIdCategoriesQueryHandler(ICategoryRepository dbContext,
    ILogger<UpdateCategoryCommand> logger,
    IGetDomainIdCategoryRepository identityService) : IRequestHandler<GetByIdCategoriesQuery, ResponseType<CategoryResponseDto>>
{
    public async Task<ResponseType<CategoryResponseDto>> Handle(GetByIdCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoriesResponse = await identityService.GetCurrentUserDomainIdGetByIdCategoryAsync(request.id);
        if (!categoriesResponse.Success)
        {
            logger.LogWarning("Failed to retrieve DomainUserId && List of Category, category validation failed: {Message}", categoriesResponse.Message);
            return ResponseType<CategoryResponseDto>.Fail(categoriesResponse.Message);
        }
        var domainUserId =  categoriesResponse.Data;
        try
        {
            var categories = dbContext.GetByIdAsync(domainUserId.UserId).Result;
            logger.LogInformation("Successfully deleted category {categoryId}", domainUserId.UserId);
            return ResponseType<CategoryResponseDto>.SuccessResult(
                new CategoryResponseDto(categories),
                "Category retrieved successfully");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error retrieving category {categoryId}", domainUserId.UserId);
            return ResponseType<CategoryResponseDto>.Fail(
                ex.Message,
                "Failed to delete category");
        }
        
    }
}