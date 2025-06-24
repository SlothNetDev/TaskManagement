

namespace TaskManagement.Infrastructures.Identity.Models
{
    /// <summary>
    /// Uses to makes token for more longer time
    /// </summary>
    public class RefreshToken
    {
         public int Id { get; set; } // Primary key
         public string Token { get; set; } = string.Empty; // Actual refresh token string
         public DateTime Expires { get; set; } // Expiration date
         public bool IsExpired => DateTime.UtcNow >= Expires; //expression embodied property
         
         public DateTime Created { get; set; }
         public string CreatedByIp { get; set; } = string.Empty;
         
         public DateTime? Revoked { get; set; }
         public string? RevokedByIp { get; set; }
         
         public bool IsActive => Revoked == null && !IsExpired; //returns true if both was true return true
         
         // Navigation
         public string UserId { get; set; } = string.Empty;
         public ApplicationUsers User { get; set; } = null!;
    }
}
