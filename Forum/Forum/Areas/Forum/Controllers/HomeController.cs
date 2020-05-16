using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.DataAccess.Data;
using Forum.DataAccess.Helpers;
using Forum.DataAccess.Repository;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Services;
using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Areas.Admin.Controllers
{
    [Area("Forum")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uniofWork;
        private readonly ApplicationDbContext _db;                  

        public HomeController(IUnitOfWork uniofWork,
            ApplicationDbContext db)
        {
            _uniofWork = uniofWork;
            _db = db;
        }

        public async Task<IActionResult> Index(PostSpecParams postParams)
        {
            // using Specification
            var spec = new PostWithSpecification(postParams);

            var countSpecification = new PostWithFiltersForCountSpecification(postParams);
            var totalItems = await _uniofWork.Post.CountAsync(countSpecification);

            var pagesLast = (int)Math.Ceiling((double)totalItems / (double)postParams.PageSize);
            var pages = Enumerable.Range(1, pagesLast).ToList();

            // запрос - если номер страница больше максимального числа - срабатывает когда гуляли по пагинации и выбрали категорию
            if(postParams.PageIndex > pagesLast)
            {
                postParams.PageIndex = pagesLast;
                return RedirectToAction("Index", new
                {
                    categoryId = postParams.CategoryId,
                    pageIndex = postParams.PageIndex,
                    pageSize = postParams.PageSize,
                    search = postParams.Search
                });
            }

            var obj = await _uniofWork.Post.GetListAsyncWithSpec(spec);
            foreach (var item in obj)
            {
                item.ApplicationUser = _db.ApplicationUsers.Find(item.ApplicationUserId);
            }
            var data = obj;
            

            return View(new Pagination<Post>(postParams.PageIndex, postParams.PageSize, totalItems, pagesLast, pages, data));
        }

        // GET: Forum/Home/Details/5
        public IActionResult Details(int id)
        {
            // using Specification
            //var spec = new PostWithSpecification(id);
            //var post = await _context.GetByIdAsyncWithSpec(spec);

            var post = _uniofWork.Post.GetByIdAsyncWithComment(id);
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

            var post = _uniofWork.Post.GetByIdAsyncWithComment(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                    ApplicationUserId = claim.Value,
                });

                _uniofWork.Post.UpdateAsync(post);
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
                _uniofWork.Post.AddSubComment(comment);
            }

            await _uniofWork.SaveChangesAsync();

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
        public async Task<IActionResult> MainCommentUpdate(CommentVM vm)
        {
            if (!ModelState.IsValid)
                // redirect to same post
                return RedirectToAction("Details", new { id = vm.PostId });

            if (ModelState.IsValid)
            {
                var comment = await _db.MainComments.FindAsync(vm.MainCommentId);

                if (comment == null)
                    return RedirectToAction("Details", new { id = vm.PostId });

                // access to edit have admin, moderator and post author
                bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
                if (!result)
                    return new RedirectResult("~/Identity/Account/AccessDenied");

                comment.Message = vm.Message;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = vm.PostId });
        }


        [HttpPost]
        public async Task<IActionResult> SubCommentUpdate(SubComment vm)
        {
            var mainPostId = vm.MainCommentId;
            var mainComm = _db.MainComments.Find(mainPostId);
            var post = _db.Posts.Where(s => s.MainComments.Select(m => m.Id).Contains(mainComm.Id));
            var postId = post.FirstOrDefault().Id;

            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = postId });

            if (ModelState.IsValid)
            {
                var comment = await _db.SubComments.FindAsync(vm.Id);

                if (comment == null)
                    return RedirectToAction("Details", new { id = postId });

                // access to edit have admin, moderator and post author
                bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
                if (!result)
                    return new RedirectResult("~/Identity/Account/AccessDenied");

                comment.Message = vm.Message;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = postId });
        }
    }
}
