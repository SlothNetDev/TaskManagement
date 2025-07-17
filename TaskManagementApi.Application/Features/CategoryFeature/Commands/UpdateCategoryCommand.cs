using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Core.IRepository.Task;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record UpdateCategoryCommand(CategoryUpdateDto dto): IRequest<ResponseType<CategoryResponseDto>>;

    public class UpdateCategoryCommandHandler(ICategoryRepository service,
        ILogger<UpdateCategoryCommand> logger,
        IGetDomainIdCategoryRepository identityService) : IRequestHandler<UpdateCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public async Task<ResponseType<CategoryResponseDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
             // 1. Validate request
            var validationErrors = ModelValidation.ModelValidationResponse(request.dto);
            if (validationErrors.Any())
            {
                logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}",
                    "PUT /category",
                    validationErrors);
                return ResponseType<CategoryResponseDto>.Fail(validationErrors, "Invalid input. Please check the provided data");
            }

            // 2. Get and validate user from JWT
            var categoryResponse = await identityService.GetCurrentUserDomainIdUpdateCategoryAsync(request.dto.Id);
            if (!categoryResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", categoryResponse.Message);
                return ResponseType<CategoryResponseDto>.Fail(categoryResponse.Message);
            }
            var categoryToUpdate = categoryResponse.Data;
            // 6. Update category
            try
            {
                // Apply updates (using null-coalescing for optional fields)
                categoryToUpdate.CategoryName = request.dto.CategoryName ?? categoryToUpdate.CategoryName;
                categoryToUpdate.Description = request.dto.Description ?? categoryToUpdate.Description;

                await service.UpdateAsync(categoryToUpdate);

                logger.LogInformation("Category {categoryId} updated successfully by user {userId}",
                    categoryToUpdate.Id, categoryToUpdate.UserId);

                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(categoryToUpdate),
                    "Category updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update category {categoryId}", request.dto.Id);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message,
                    "Failed to update category");
            }
        }
    }
}
