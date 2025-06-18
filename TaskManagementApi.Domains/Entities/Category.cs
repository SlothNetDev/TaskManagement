
using System.ComponentModel.DataAnnotations;
using TaskManagement.Infrastructures.Identity;

namespace TaskManagementApi.Domains.Entities
{
    /// <summary>
    /// Model for Categories
    /// </summary>
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        /*[DisplayName("Category Name")]*/
        [StringLength(120/*, ErrorMessage = "{0} Cannot exceed with 120 characters"*/)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(1000/*, ErrorMessage = "Descriptions Cannot exceed with 1000 characters"*/)]
        public string? Description { get; set; }
    
        // Foreign key and navigation to User
        public Guid UserId { get; set; }
        public Users User { get; set; }

        // Navigation to related Tasks
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    }

}
