using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    public class AuthResultDto
    {
        [JsonPropertyName("bearerToken")]
        public string BearerToken { get; set; }
        
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
