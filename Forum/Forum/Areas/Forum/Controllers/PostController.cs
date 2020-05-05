using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.DataAccess.Repository.IRepository;

namespace Forum.Areas.Forum.Controllers
{
    [Area("Forum")]
    public class PostController : Controller
    {
        private readonly IGenericRepository<Post> _context;

        public PostController(IGenericRepository<Post> context)
        {
            _context = context;
        }

        // GET: Forum/Post
        public async Task<IActionResult> Index()
        {
            return View(await _context.GetListAsync());
        }

        // GET: Forum/Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var posts = await _context.GetByIdAsync(id);
            if (posts == null)
                return NotFound();
            return View(posts);
        }

        // GET: Forum/Post/Create
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Post());
            else
                return View(await _context.GetByIdAsync(id));
        }

        // POST: Forum/Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Title,Body,ImageUrl,Created,Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.Id == 0)
                    await _context.CreateAsync(post);
                else
                    _context.UpdateAsync(post);
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
