using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using Forum.Models.ViewModels;
using Forum.Utility.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;




namespace Forum.Areas.Admin.Controllers
{
    [Area("Forum")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;                  
        private readonly IGenericRepository<Post> _context;

        public HomeController(ILogger<HomeController> logger,
            IGenericRepository<Post> context,
            ApplicationDbContext db)
        {
            _logger = logger;
            _context = context;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // using Specification
            var spec = new PostWithSpecification();
            var obj = await _context.GetListAsyncWithSpec(spec);
            foreach (var item in obj)
            {
                item.ApplicationUser = _db.ApplicationUsers.Find(item.ApplicationUserId);
            }
            return View(obj);
        }

        // GET: Forum/Home/Details/5
        public IActionResult Details(int id)
        {
            // using Specification
            //var spec = new PostWithSpecification(id);
            //var post = await _context.GetByIdAsyncWithSpec(spec);

            var post = _context.GetByIdAsyncWithComment(id);
            if (post == null)
                return NotFound();
            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // comment section
        [HttpPost]
        public async Task<IActionResult> Comment(CommentVM vm)
        {
            if (!ModelState.IsValid)
                // redirect to same post
                return RedirectToAction("Details", new { id = vm.PostId });

            // get logged user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var post = _context.GetByIdAsyncWithComment(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                    ApplicationUserId = claim.Value,
                });

                _context.UpdateAsync(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                    ApplicationUserId = claim.Value,
                };
                _context.AddSubComment(comment);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = vm.PostId });
        }

        // Delete main comments and all subcomments
        [HttpDelete]
        public async Task<IActionResult> DeleteMainComment(int id)
        {
            var comment = await _db.MainComments.FindAsync(id);
            if (comment == null)
                return Json(new { success = false, message = "Error while deleting" });

            // Check user permissions
            bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
            if(!result)
                return Json(new { success = false, message = "Access Denied. You do not have rights for deleting." });

            //var allSub = _db.SubComments.ToList();
            //allSub.RemoveAll(s => s.MainCommentId == id);
            var subComments = _db.SubComments.Where(s => s.MainCommentId == id).ToList();
            _db.SubComments.RemoveRange(subComments);

            _db.MainComments.Remove(comment);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        // Delete main comments and all subcomments
        [HttpDelete]
        public async Task<IActionResult> DeleteSubComment(int id)
        {
            var comment = await _db.SubComments.FindAsync(id);
            if (comment == null)
                return Json(new { success = false, message = "Error while deleting" });

            // Check user permissions
            bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
            if (!result)
                return Json(new { success = false, message = "Access Denied. You do not have rights for deleting." });

            _db.SubComments.Remove(comment);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }



        [HttpPost]
        public async Task<IActionResult> UpdateMainComment(string ApplicationUserId, string Id, string Message)
        {
            var body = new StreamReader(Request.Body);

            using var reader = new StreamReader(HttpContext.Request.Body);
            var body2 = await reader.ReadToEndAsync();

            //var dt = await _db.MainComments.FindAsync(comm.Id);
            //var comment = await _db.MainComments.FindAsync(commentId);

            //if (comment == null)
            //    return Json(new { success = false, message = "Error while updating" });

            //// Check user permissions
            //bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
            //if (!result)
            //    return Json(new { success = false, message = "Access Denied. You do not have rights for deleting." });

            //if (comment == null)
            //    comment.Message = message;

            //await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Update Successful" });
        }
    }
}
