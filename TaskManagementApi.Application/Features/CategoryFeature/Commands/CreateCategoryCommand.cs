using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record CreateCategoryCommand(CategoryRequestDto Dto) :IRequest<ResponseType<CategoryResponseDto>>;

    public class CreateCategoryCommandHandler(ICategoryRepository dbContext,
        IGetDomainIdCategoryRepository identityService,
        ILogger<CreateCategoryCommandHandler> logger) : IRequestHandler<CreateCategoryCommand, ResponseType<CategoryResponseDto>>
    {
        public async Task<ResponseType<CategoryResponseDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
                // 1. Validate request
            var validationErrors = ModelValidation.ModelValidationResponse(request.Dto);
            if (validationErrors.Any())
            {
                logger.LogWarning("Request validation failed for {Endpoint}. Errors: {@ValidationErrors}", 
                    "POST /category", 
                    validationErrors);
                return ResponseType<CategoryResponseDto>.Fail(validationErrors, "Invalid input. Please check the provided data");
            }
            
            //2. Get UserDomain Id
            var userDomainResponse =  await identityService.GetCurrentUserDomainIdCreateCategoryAsync();
            if (!userDomainResponse.Success)
            {
                logger.LogWarning("Failed to retrieve DomainUserId or category validation failed: {Message}", userDomainResponse.Message);
                return ResponseType<CategoryResponseDto>.Fail(userDomainResponse.Message);
            }
            var userDomainId = userDomainResponse.Data;
            try
            {
                //3. Create category
                var category = new Category
                {
                    UserId = userDomainId,
                    Id = Guid.NewGuid(),
                    CategoryName = request.Dto.CategoryName,
                    Description = request.Dto.Description
                };
                
                //4. Create ans Save Category
                await dbContext.CreateAsync(category);
        
                logger.LogInformation("Category {categoryId} created successfully for user {userId}", 
                    category.Id, userDomainId);
        
                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(category),
                    "Category created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create category for user {userId}", userDomainId);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message, 
                    "Failed to create category");
            }
        }
    }
}
