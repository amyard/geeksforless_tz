﻿using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Authorization;
using Forum.Utility.Services;


namespace Forum.Areas.Forum.Controllers
{
    [Area("Forum")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;                   // override to repo
        private readonly IGenericRepository<Post> _context;
        private readonly IFileManager _fileManager;                  // for upload images on server

        public PostController(IGenericRepository<Post> context,
            IFileManager fileManager,
            ApplicationDbContext db)
        {
            _context = context;
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
        [Authorize()]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            //var ids = GetUserInfo();
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
                postVM.Post = await _context.GetByIdAsync(id);

                // access to edit have admin, moderator and post author
                bool result = AccessRights.AuthorAdminAccessRight(HttpContext, postVM, _db);
                if (result)
                    return View(postVM);
                return new RedirectResult("~/Identity/Account/AccessDenied");
        }

        // POST: Forum/Post/Create
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
                        Post obj = await _context.GetByIdAsync(postVM.Post.Id);
                        _fileManager.RemoveImage(obj.ImageUrl);
                    }
                    postVM.Post.ImageUrl = await _fileManager.SaveImage(files, SD.Post_Image_Base_Path, SD.Post_Image_Result_Path);
                }
                else
                {
                    // update when they do not change the image
                    if (postVM.Post.Id != 0)
                    {
                        Post objFromDb = await _context.GetByIdAsync(postVM.Post.Id);
                        postVM.Post.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                // get error if use   _context.Update(appartment);
                if (postVM.Post.Id == 0)
                {
                    // get logged user
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    postVM.Post.ApplicationUserId = claim.Value;
                    await _context.CreateAsync(postVM.Post);
                }
                else
                {
                    var objFromDb = await _context.GetByIdAsync(postVM.Post.Id);
                    if (objFromDb != null)
                    {
                        objFromDb.Title = postVM.Post.Title;
                        objFromDb.Body = postVM.Post.Body;
                        objFromDb.CategoryId = postVM.Post.CategoryId;
                        objFromDb.ImageUrl = postVM.Post.ImageUrl;
                        objFromDb.Modified = DateTime.Now;
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postVM.Post);
        }

        // GET: Forum/Post/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.GetByIdAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
             
            _fileManager.RemoveImage(post.ImageUrl);
            await _context.DeleteJsAsync(id);
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
