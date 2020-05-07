using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.DataAccess.Repository.IRepository;
using Forum.Utility;
using Forum.DataAccess.Data;
using System;
using System.Security.Claims;
using Forum.DataAccess.Specification;
using Forum.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forum.Areas.Forum.Controllers
{
    [Area("Forum")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;                   // override to repo
        private readonly IGenericRepository<Post> _context;
        private readonly IGenericRepository<Category> _category;
        private readonly IFileManager _fileManager;                  // for upload images on server

        public PostController(IGenericRepository<Post> context,
            IGenericRepository<Category> category,
            IFileManager fileManager,
            ApplicationDbContext db)
        {
            _context = context;
            _category = category;
            _fileManager = fileManager;
            _db = db;
        }

        // GET: Forum/Post
        public async Task<IActionResult> Index()
        {
            // using Specification
            var spec = new PostWithSpecification();
            return View(await _context.GetListAsyncWithSpec(spec));
        }

        // GET: Forum/Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // using Specification
            var spec = new PostWithSpecification(id);
            var post = await _context.GetByIdAsyncWithSpec(spec);
            if (post == null)
                return NotFound();
            return View(post);
        }

        // GET: Forum/Post/Create
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            PostVM postVM = new PostVM()
            {
                Post = new Post(),
                CategoryList = _db.Categories.ToList().Select(i => new SelectListItem
                {
                    Text = i.Title,
                    Value = i.Id.ToString()
                })
            };

            if (id == 0)
                return View(postVM);
            else
                return View(await _context.GetByIdAsync(id));
        }

        // POST: Forum/Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Post post)
        {
            if (ModelState.IsValid)
            {
                // SAVE IMAGE
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    // DELETE OLD IMAGE
                    if (post.Id != 0)
                    {
                        Post obj = await _context.GetByIdAsync(post.Id);
                        _fileManager.RemoveImage(obj.ImageUrl);
                    }
                    post.ImageUrl = await _fileManager.SaveImage(files, SD.Post_Image_Base_Path, SD.Post_Image_Result_Path);
                }
                else
                {
                    // update when they do not change the image
                    if (post.Id != 0)
                    {
                        Post objFromDb = await _context.GetByIdAsync(post.Id);
                        post.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                // get error if use   _context.Update(appartment);
                if (post.Id == 0)
                {
                    // get logged user
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    post.ApplicationUserId = claim.Value;
                    await _context.CreateAsync(post);
                }
                else
                {
                    var objFromDb = await _context.GetByIdAsync(post.Id);
                    if (objFromDb != null)
                    {
                        objFromDb.Title = post.Title;
                        objFromDb.Body = post.Body;
                        objFromDb.ImageUrl = post.ImageUrl;
                        objFromDb.Modified = DateTime.Now;
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/RentTerm/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.GetByIdAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _context.DeleteJsAsync(id);
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
