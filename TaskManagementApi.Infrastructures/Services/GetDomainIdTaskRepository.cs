using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services;

public class GetDomainIdTaskRepository(
    ApplicationDbContext applicationDbContext,
    IHttpContextAccessor  httpContextAccessor,
    ILogger<GetDomainIdCategoryRepository> logger) :IGetDomainTaskRepository
{
    public async Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateCategoryAsync(Guid id)
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
        //get category if it was exist in user
        var category = await applicationDbContext.CategoryDb
            .AnyAsync(x => x.Id == parseUserId);

        //validate category
        if (category is true)
        {
            logger.LogError("Expected {Category} was null when processing {Create}",
                category,
                "Create Task");
            return ResponseType<Guid>.Fail("Category don't exist to user Account");
        }

        return ResponseType<Guid>.SuccessResult(matchingApplicationUser.DomainUserId,
            "User Domain Id Retrieved Successfully");
    }

    public Task<ResponseType<Guid>> GetCurrentUserDomainIdUpdateCategoryAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}