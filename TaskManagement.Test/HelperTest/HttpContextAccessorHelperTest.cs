using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data;

namespace TaskManagement.Test.HelperTest
{
    internal static class HttpContextAccessorHelperTest
    {
        public static void SetUpHttpContextAccessor(Guid jwtUserID, Mock<IHttpContextAccessor> _mockHttpContextAccessor)
        {
            //convert jwtUserId into claims token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,jwtUserID.ToString())
            };
            //creating identity using token claims
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"))
            };
            //make httpAccessor to setUp the identity with fake token
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
                
        }
    }
}
