using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.DataAccess;
using Forum.DataAccess.Data;
using Forum.DataAccess.Helpers;
using Forum.DataAccess.Repository;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Services;
using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Areas.Admin.Controllers
{
    [Area("Forum")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uniofWork;
        private readonly ApplicationDbContext _db;
        private readonly IFileManager _fileManager;     // for upload images on server

        public HomeController(IFileManager fileManager,
            IUnitOfWork uniofWork,
            ApplicationDbContext db)
        {
            _fileManager = fileManager;
            _uniofWork = uniofWork;
            _db = db;
        }

        public string StatusMessage { get; set; }

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


        [Authorize()]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            //var ids = GetUserInfo();
            PostVM postVM = new PostVM()
            {
                Post = new Post(),
                CategoryList = _uniofWork.Category.GetSelectListAsync()
            };

            if (id == 0)
                return View(postVM);
            else
                postVM.Post = await _uniofWork.Post.GetByIdAsync(id);

            // access to edit have admin, moderator and post author
            bool result = AccessRights.AuthorAdminAccessRight(HttpContext, postVM.Post.ApplicationUserId, _db);
            if (result)
                return View(postVM);
            return new RedirectResult("~/Identity/Account/AccessDenied");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(PostVM postVM)
        {
            if (ModelState.IsValid)
            {
                // SAVE IMAGE
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    // DELETE OLD IMAGE
                    if (postVM.Post.Id != 0)
                    {
                        Post obj = await _uniofWork.Post.GetByIdAsync(postVM.Post.Id);
                        _fileManager.RemoveImage(obj.ImageUrl);
                    }
                    postVM.Post.ImageUrl = await _fileManager.SaveImage(files, SD.Post_Image_Base_Path, SD.Post_Image_Result_Path);
                }
                else
                {
                    // update when they do not change the image
                    if (postVM.Post.Id != 0)
                    {
                        Post objFromDb = await _uniofWork.Post.GetByIdAsync(postVM.Post.Id);
                        postVM.Post.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (postVM.Post.Id == 0)
                {
                    // get logged user
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    postVM.Post.ApplicationUserId = claim.Value;
                    await _uniofWork.Post.CreateAsync(postVM.Post);
                }
                else
                {
                    _uniofWork.Post.UpdateAsync(postVM.Post);
                }
                await _uniofWork.SaveChangesAsync();
                var ids = _db.Posts.Where(p => p.Title == postVM.Post.Title).ToList().Last().Id;
                return RedirectToAction("Details", new { id = ids });
            }
            return View(postVM.Post);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _uniofWork.Post.GetByIdAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // delete images and comments -> post
            _fileManager.RemoveImage(post.ImageUrl);
            _uniofWork.Post.DeleteAllCommentByPostId(id);
            await _uniofWork.Post.DeleteJsAsync(id);
            return Json(new { success = true, message = "Delete Successful" });
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

            if (vm.MainCommentId == 0)
                await _uniofWork.MainComment.AddCommentFromCommentView(vm, claim);
            else
                await _uniofWork.SubComment.AddCommentFromCommentView(vm, claim);

            await _uniofWork.SaveChangesAsync();

            return RedirectToAction("Details", new { id = vm.PostId });
        }


        // Delete main comments and all subcomments
        [HttpDelete]
        public async Task<IActionResult> DeleteMainComment(int id)
        {
            var comment = await _uniofWork.MainComment.GetByIdAsync(id);
            if (comment == null)
                return Json(new { success = false, message = "Error while deleting" });

            // Check user permissions
            bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
            if(!result)
                return Json(new { success = false, message = "Access Denied. You do not have rights for deleting." });

            await _uniofWork.MainComment.DeleteMainAndSubComments(id);    
            await _uniofWork.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        // Delete main comments and all subcomments
        [HttpDelete]
        public async Task<IActionResult> DeleteSubComment(int id)
        {
            var comment = await _uniofWork.SubComment.GetByIdAsync(id);
            if (comment == null)
                return Json(new { success = false, message = "Error while deleting" });

            // Check user permissions
            bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
            if (!result)
                return Json(new { success = false, message = "Access Denied. You do not have rights for deleting." });

            await _uniofWork.SubComment.DeleteComment(id);
            await _uniofWork.SaveChangesAsync();

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
                var comment = await _uniofWork.MainComment.GetByIdAsync(vm.MainCommentId);

                if (comment == null)
                    return RedirectToAction("Details", new { id = vm.PostId });

                // access to edit have admin, moderator and post author
                bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
                if (!result)
                    return new RedirectResult("~/Identity/Account/AccessDenied");

                comment.Message = vm.Message;
                await _uniofWork.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = vm.PostId });
        }


        [HttpPost]
        public async Task<IActionResult> SubCommentUpdate(SubComment vm)
        {
            var post = _uniofWork.Post.GetPostByMainCommentId(vm.MainCommentId);

            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = post.Id });

            if (ModelState.IsValid)
            {
                var comment = await _uniofWork.SubComment.GetByIdAsync(vm.Id);

                if (comment == null)
                    return RedirectToAction("Details", new { id = post.Id });

                // access to edit have admin, moderator and post author
                bool result = AccessRights.AuthorAdminAccessRight(HttpContext, comment.ApplicationUserId, _db);
                if (!result)
                    return new RedirectResult("~/Identity/Account/AccessDenied");

                comment.Message = vm.Message;
                await _uniofWork.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = post.Id });
        }
    }
}
