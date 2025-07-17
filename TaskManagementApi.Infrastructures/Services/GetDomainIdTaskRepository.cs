using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.ApplicationHelpers;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Domains.Entities;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services;

public class GetDomainIdTaskRepository(
    ApplicationDbContext applicationDbContext,
    IHttpContextAccessor  httpContextAccessor,
    ILogger<GetDomainIdCategoryRepository> logger) :IGetDomainTaskRepository
{
    #region  Task Command 

    public async Task<ResponseType<Guid>> GetCurrentUserDomainIdCreateTaskAsync()
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
    
    public async Task<ResponseType<TaskItem>> GetCurrentUserDomainIdUpdateTaskAsync(Guid id)
    {
        // 1. Validate request
        var validationErrors = ModelValidation.ModelValidationResponse(id);
        if (validationErrors.Any())
        {
            logger.LogWarning("UT_001: Request validation failed. Errors: {@ValidationErrors}", 
                validationErrors);
            return ResponseType<TaskItem>.Fail(
                validationErrors,
                "Invalid task data. Please correct the highlighted fields");
        }
        
        // 2. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            logger.LogWarning("UT_002: Invalid user token {UserId}", userId);
            return ResponseType<TaskItem>.Fail(
                "Authentication failed",
                "Invalid user credentials");
        }
        
        // 3. Match application user
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("UT_003: User profile not found {UserId}", parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "User profile not found",
                "Please complete your account setup");
        }
        
        // 4. Validate domain user ID
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("UT_004: Missing domain ID for user {UserId}", parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "Account configuration incomplete",
                "Missing domain user ID");
        }
        
        // 5. Verify user has at least one category
        var hasCategory = await applicationDbContext.CategoryDb
            .AnyAsync(x => x.UserId == taskUserIdToUse);
        
        if (!hasCategory)
        {
            logger.LogWarning("UT_005: No categories found for user {UserId}", parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "No categories available",
                "Please create a category first");
        }
        
        // 6. Find and validate task
        var taskToUpdate = await applicationDbContext.TaskDb
            .FirstOrDefaultAsync(x => x.UserId == taskUserIdToUse && x.Id == id);
        
        if (taskToUpdate == null)
        {
            logger.LogWarning("UT_006: Task {TaskId} not found for user {UserId}", 
                id, parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "Task not found",
                "The specified task doesn't exist or you don't have permission");
        }
        return ResponseType<TaskItem>.SuccessResult(taskToUpdate,
            "User Domain Id Retrieved Successfully");
    }

    public async Task<ResponseType<TaskItem>> GetCurrentUserDomainIdDeleteTaskAsync(Guid id)
    {
        // 1. Validate task ID
        if (id == Guid.Empty)
        {
            logger.LogWarning("DT_001: Attempted to delete task with empty ID");
            return ResponseType<TaskItem>.Fail(
                "Invalid task identifier",
                "Task ID cannot be empty");
        }
        
        // 2. Get and validate user from JWT
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            logger.LogWarning("DT_002: Failed to extract valid user ID from JWT. Provided ID: {UserId}", userId);
            return ResponseType<TaskItem>.Fail(
                "Authentication failed",
                "Invalid user credentials");
        }
        
        // 3. Match application user
        logger.LogInformation("DT_DEBUG: Attempting task deletion for UserId: {UserId}", parsedUserId);
        var matchingApplicationUser = await applicationDbContext.UserApplicationDb
            .FirstOrDefaultAsync(ac => ac.Id == parsedUserId);
        
        if (matchingApplicationUser == null)
        {
            logger.LogWarning("DT_003: No user profile found for ID {UserId}", parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "User profile not found",
                "Invalid user account");
        }
        
        // 4. Validate domain user ID
        var taskUserIdToUse = matchingApplicationUser.DomainUserId;
        if (taskUserIdToUse == Guid.Empty)
        {
            logger.LogWarning("DT_004: Empty domain ID for user {UserId}", parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "User configuration incomplete",
                "Missing domain user ID");
        }
        
        // 5. Find and validate task
        var taskToDelete = await applicationDbContext.TaskDb
            .FirstOrDefaultAsync(t => t.UserId == taskUserIdToUse && t.Id == id);
        
        if (taskToDelete == null)
        {
            logger.LogWarning("DT_005: Task {TaskId} not found for user {UserId}", id, parsedUserId);
            return ResponseType<TaskItem>.Fail(
                "Task not found",
                "The specified task doesn't exist or you don't have permission");
        }
        return ResponseType<TaskItem>.SuccessResult(
            taskToDelete,
            "User Domain Id Retrieved Successfully");
    }

    

    #endregion

    #region TaskQuery
    public async Task<ResponseType<Guid>> GetCurrentUserDomainIdGetAllTaskAsync()
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

    public Task<ResponseType<TaskItem>> GetCurrentUserDomainIdGetByIdTaskAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseType<TaskItem>> GetCurrentUserDomainIdPaganationTaskAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseType<TaskItem>> GetCurrentUserDomainIdSearchTaskAsync(string id)
    {
        throw new NotImplementedException();
    }

    #endregion
}