using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.DTOs;

namespace TaskManagement.Infrastructures.Identity.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _setting;
        public TokenService(JwtSettings setting)
        {
            _setting = setting;
        }
        public Task<string> GenerateTokenAsync(TokenUserDto user)
        {
            var claim = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId) // Backward compatibility
            };
            claim.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.Key));
            var credits = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _setting.Issuer,
                audience: _setting.Audience,
                expires: DateTime.UtcNow.AddDays(7),
                claims: claim,
                signingCredentials: credits);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));


        }
    }
}
