using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services;

public class GetDomainIdCategoryRepository(
    ApplicationDbContext applicationDbContext,
    IHttpContextAccessor  httpContextAccessor,
    ILogger<GetDomainIdCategoryRepository> logger
    ) : IGetDomainIdCategoryRepository
{
    
    public async Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateCategoryAsync()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
        {
            logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
            return ResponseType<Guid>.Fail("Unauthorized or invalid user.");
        }
        
        logger.LogInformation("Attempting to create category for UserId: {UserId}", parseUserId);
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(x => x.Id == parseUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
            return ResponseType<Guid>.Fail("Invalid user account");
        }
        
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
            return ResponseType<Guid>.Fail("Invalid user configuration");
        }

        return ResponseType<Guid>.SuccessResult(matchingApplicationUser.DomainUserId,
            "User Domain Id Retrieved Successfully");
    }

    public async Task<ResponseType<Category>> GetCurrentUserDomainIdUpdateCategoryAsync(Guid id)
    {
        // 2. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
        {
            logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
            return ResponseType<Category>.Fail("Unauthorized or invalid user");
        }

        // 3. Match application user
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parseUserId);

        if (matchingApplicationUser == null)
        {
            logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user account");
        }

        // 4. Validate domain user ID
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user configuration");
        }

        // 5. Find and validate category
        var categoryToUpdate = await applicationDbContext.CategoryDb
            .FirstOrDefaultAsync(c => c.UserId == taskUserIdToUse && c.Id == id);

        if (categoryToUpdate == null)
        {
            logger.LogDebug("Category not found for UserId: {UserId}, CategoryId: {CategoryId}",
                taskUserIdToUse, id);
            return ResponseType<Category>.Fail("Category not found");
        }
        return ResponseType<Category>.SuccessResult(categoryToUpdate,
            "User Domain Id Retrieved Successfully");
    }

    public async Task<ResponseType<Category>> GetCurrentUserDomainIdDeleteCategoryAsync(Guid id)
    {
        // 1. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
        {
            logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
            return ResponseType<Category>.Fail("Unauthorized or invalid user.");
        }
        // 2. Match application user
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user account.");
        }
        
        // 3. Validate domain user ID
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user configuration.");
        }
        
        //check if Id exist
        if (id == Guid.Empty)
        {
            logger.LogWarning("Delete {id} is not exist.", id);
            return ResponseType<Category>.Fail("Invalid Id, It's not Exist");
        }
        // 4. Find and validate category
        var categoryToDelete = await applicationDbContext.CategoryDb
            .FirstOrDefaultAsync(c => c.UserId == taskUserIdToUse && c.Id == id);
        
        if (categoryToDelete == null)
        {
            logger.LogWarning("No category found with ID {categoryId} for user {userId}.", 
                id, parseUserId);
            return ResponseType<Category>.Fail("Category not found.");
        }
        
        return ResponseType<Category>.SuccessResult(
            categoryToDelete,
            "User Domain Id Retrieved Successfully");
    }

    public async Task<ResponseType<Guid>> GetCurrentUserDomainIdGetAllCategoryAsync()
    {
        // 1. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            logger.LogWarning("GA_CAT_001: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
            return ResponseType<Guid>.Fail(
                "Authentication failed. Invalid user identifier.");
        }
        
        // 2. Match application user
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("GA_CAT_002: No matching ApplicationUser found for userId {ParsedUserId}.", parsedUserId);
            return ResponseType<Guid>.Fail(
                "User profile not found.");
        }

        if (matchingApplicationUser.DomainUserId == Guid.Empty)
        {
            logger.LogWarning("AUTH_003: User {UserId} has an empty DomainUserId", parsedUserId);
            return ResponseType<Guid>.Fail("Invalid user configuration");
        }
        return ResponseType<Guid>.SuccessResult(matchingApplicationUser.DomainUserId,
            "User Domain Id Retrieved Successfully");
    }

    public async Task<ResponseType<Category>> GetCurrentUserDomainIdGetByIdCategoryAsync(Guid id)
    {
        // 1. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parseUserId))
        {
            logger.LogWarning("Failed to extract valid user ID {id} from JWT.", userId);
            return ResponseType<Category>.Fail("Unauthorized or invalid user.");
        }
        // 2. Match application user
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parseUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("No matching ApplicationUser found for userId {id}.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user account.");
        }
        
        // 3. Validate domain user ID
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("User {id} has an empty DomainUserId.", parseUserId);
            return ResponseType<Category>.Fail("Invalid user configuration.");
        }
        
        //check if Id exist
        if (id == Guid.Empty)
        {
            logger.LogWarning("Delete {id} is not exist.", id);
            return ResponseType<Category>.Fail("Invalid Id, It's not Exist");
        }
        // 4. Find and validate category
        var category = await applicationDbContext.CategoryDb
            .FirstOrDefaultAsync(c => c.UserId == taskUserIdToUse && c.Id == id);
        
        if (category == null)
        {
            logger.LogWarning("No category found with ID {categoryId} for user {userId}.", 
                id, parseUserId);
            return ResponseType<Category>.Fail("Category not found.");
        }
        
        return ResponseType<Category>.SuccessResult(
            category,
            "User Domain Id Retrieved Successfully");
    }
}