using Microsoft.AspNetCore.Http;

namespace TaskManagement.Infrastructures.InfrustructureHelper
{
    internal class IPadressHelper(IHttpContextAccessor _httpContextAccessor)
    {
        public  string GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        }
           
    }
}
