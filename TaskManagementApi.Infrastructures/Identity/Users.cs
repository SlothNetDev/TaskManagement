using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Infrastructures.Identity
{
    //Models for User, User Account 
    public class Users : IdentityUser<Guid>
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    
        // Navigation properties
       /* public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();*/
    }

}
