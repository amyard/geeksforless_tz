using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Forum.Utility.Services
{
    public class GetCurrentUser
    {
        public static string GetData(HttpContext httpContext)
        {
            return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // return httpContext.User.Identity.Name;
        }
    }
}
