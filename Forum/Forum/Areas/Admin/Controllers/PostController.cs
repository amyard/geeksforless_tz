using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Data;
using System;
using System.Security.Claims;
using Forum.DataAccess.Specification;
using Forum.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Forum.DataAccess.Services;
using Forum.DataAccess;
using Forum.DataAccess.Repository;

namespace Forum.Areas.Forum.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly IUnitOfWork _uniofWork;
        private readonly ApplicationDbContext _db;                  
        private readonly IFileManager _fileManager;     // for upload images on server

        public PostController(IFileManager fileManager,
            ApplicationDbContext db,
            IUnitOfWork uniofWork)
        {
            _fileManager = fileManager;
            _db = db;
            _uniofWork = uniofWork;
        }

        // GET: Admin/Post
        public async Task<IActionResult> Index()
        {
            return View(await _uniofWork.Post.GetListAsync());
        }

        // GET: Admin/Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // using Specification
            var spec = new PostWithSpecification(id);
            var post = await _uniofWork.Post.GetByIdAsyncWithSpec(spec);
            if (post == null)
                return NotFound();
            return View(post);
        }

        // GET: Admin/Post/Create
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

        // POST: Admin/Post/Create
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
                    var objFromDb = await _uniofWork.Post.GetByIdAsync(postVM.Post.Id);
                    if (objFromDb != null)
                    {
                        objFromDb.Title = postVM.Post.Title;
                        objFromDb.Body = postVM.Post.Body;
                        objFromDb.CategoryId = postVM.Post.CategoryId;
                        objFromDb.ImageUrl = postVM.Post.ImageUrl;
                        objFromDb.Modified = DateTime.Now;
                    }
                }
                await _uniofWork.Post.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postVM.Post);
        }

        // GET: Admin/Post/Delete/5
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

        // get API for admin
        [HttpGet]
        public IActionResult GetPostForAdmin()
        {
            var objList = _db.Posts.ToList();
            foreach(var item in objList)
            {
                item.Category = _db.Categories.Find(item.CategoryId);
                item.ApplicationUser = _db.ApplicationUsers.Find(item.ApplicationUserId);
            }
            return Json(new { data = objList });
        }
    }
}
