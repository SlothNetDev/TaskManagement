using Microsoft.AspNetCore.Identity;
using TaskManagementApi.Domains.Entities;
namespace TaskManagement.Infrastructures.Identity
{
    //Models for User, User Account 
    public class ApplicationUsers : IdentityUser<Guid> //identity user Id set as Guid
    {
        // This class primarily inherits properties from IdentityUser (like Id, UserName, Email, PasswordHash, etc.)
        // You would add any *additional* properties here that are specific to your *Identity/Authentication* needs
        // but are NOT core domain concepts (e.g., FirstName, LastName, ProfilePictureUrl if stored here).

        // You should NOT add navigation properties to Tasks or Categories directly here.
        // Instead, the link is established in ApplicationDbContext via the Domain.Users entity.
        // If you were to add them here, they would point to your Domain.Entities classes, which is fine,
        // but the recommended way to bridge is via the Domain.Users entity.
       
        // The critical link to your Domain User:
        public Guid DomainUserId { get; set; } // Foreign key to the Domain.Users Id
        public Users DomainUser { get; set; } = null!; // Navigation property to your Domain.Users entity
    }

}
