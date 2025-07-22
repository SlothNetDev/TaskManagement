using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Application.Features.CategoryFeature.Commands;
using TaskManagementApi.Application.Features.Task.TaskDto;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Wrapper;
namespace TaskManagementApi.Application.Features.CategoryFeature.Queries
{
    public record GetAllCategoriesQuery() : IRequest<ResponseType<List<CategoryResponseDtoWithTask>>>;
    public class GetAllCategoriesQueryHanlder(ICategoryRepository service,
        ILogger<UpdateCategoryCommand> logger,
        IGetDomainIdCategoryRepository identityService) : IRequestHandler<GetAllCategoriesQuery, ResponseType<List<CategoryResponseDtoWithTask>>>
    {
        public async Task<ResponseType<List<CategoryResponseDtoWithTask>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categoriesResponse = await identityService.GetCurrentUserDomainIdGetAllCategoryAsync();
            if (!categoriesResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId && List of Category, category validation failed: {Message}", categoriesResponse.Message);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(categoriesResponse.Message);
            }
            var domainUserId = categoriesResponse.Data;
            try
            {
                var categories = await service.GetByAllCategoryAsync(domainUserId);
                
                var categoriesdto = categories
                    .Select(c => new CategoryResponseDtoWithTask(
                        c.Id,
                        c.CategoryName,
                        c.Description ?? string.Empty,
                        c.Tasks.Select(t => new TaskResponseDto(t)).ToList()))
                    .ToList();
        
                if (!categories.Any())
                {
                    logger.LogInformation("GA_CAT_004: No Categories found for user {ParsedUserId}.", domainUserId);
                    return ResponseType<List<CategoryResponseDtoWithTask>>.SuccessResult(
                        new List<CategoryResponseDtoWithTask>(),
                        "No categories found. Consider creating one.");
                }
        
                logger.LogInformation("Successfully retrieved {Count} categories for user {UserId}", 
                    categories.Count, domainUserId);
                
                return ResponseType<List<CategoryResponseDtoWithTask>>.SuccessResult(
                    categoriesdto,
                    "Successfully retrieved all categories.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GA_CAT_005: Failed to retrieve categories for user {ParsedUserId}", domainUserId);
                return ResponseType<List<CategoryResponseDtoWithTask>>.Fail(
                    ex.Message,
                    "Failed to retrieve categories. Please try again later.");
            }
        }
    }
}
