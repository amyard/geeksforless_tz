using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Forum.DataAccess.Data;
using System.Linq;

namespace Forum.Utility.Services
{
    public class AccessRights
    {

        public static bool AuthorAdminAccessRight(HttpContext httpContext, PostVM postVM, ApplicationDbContext db)
        {
            var userId = GetCurrentUser.GetData(httpContext);
            var user = db.ApplicationUsers.Find(userId);
            var userRole = db.UserRoles.ToList();
            var roles = db.Roles.ToList();
            var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            if (postVM.Post.ApplicationUserId == userId || 
                SD.Role_Admin == user.Role || SD.Role_Moderator == user.Role)
                return true;
            return false;
        }
    }
}
