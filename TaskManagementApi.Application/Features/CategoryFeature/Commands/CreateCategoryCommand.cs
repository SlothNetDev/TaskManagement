using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Core.Interfaces;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Core.IRepository.User;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Features.CategoryFeature.Commands
{
    public record CreateCategoryCommand(CategoryRequestDto Dto) :IRequest<ResponseType<CategoryResponseDto>>;

    public class CreateCategoryCommandHandler(ICategoryRepository dbContext,
        IAuthRepository identityService,
        ILogger<CreateCategoryCommandHandler> logger,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateCategoryCommand, ResponseType<CategoryResponseDto>>
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
            var userDomainId =  await identityService.GetCurrentUserDomainIdAsync();
            
            try
            {
                var category = new Category
                {
                    UserId = userDomainId.Data.Value,
                    Id = Guid.NewGuid(),
                    CategoryName = request.Dto.CategoryName,
                    Description = request.Dto.Description
                };
        
                await dbContext.CreateAsync(category);
        
                logger.LogInformation("Category {categoryId} created successfully for user {userId}", 
                    category.Id, userDomainId.Data.Value);
        
                return ResponseType<CategoryResponseDto>.SuccessResult(
                    new CategoryResponseDto(category),
                    "Category created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create category for user {userId}", userDomainId.Data.Value);
                return ResponseType<CategoryResponseDto>.Fail(
                    ex.Message, 
                    "Failed to create category");
            }
        }
    }
}
