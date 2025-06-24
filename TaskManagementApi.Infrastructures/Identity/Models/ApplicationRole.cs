using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace TaskManagement.Infrastructures.Identity.Models
{
    public class ApplicationRole :IdentityRole<Guid>
    {
        //provide roles
        public string Description { get; set; } = string.Empty;

        //used for time stamp
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
