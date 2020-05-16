using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.DataAccess;
using Forum.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Forum.DataAccess.Repository;

namespace Forum.Areas.Forum.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _uniofWork;

        public CategoryController(IUnitOfWork uniofWork)
        {
            _uniofWork = uniofWork;
        }

        // GET: Forum/Category
        public async Task<IActionResult> Index()
        {
            return View(await _uniofWork.Category.GetListAsync());
        }

        // GET: Forum/Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _uniofWork.Category.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // GET: Forum/Category/Create
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Category());
            else
                return View(await _uniofWork.Category.GetByIdAsync(id));
        }

        // POST: Forum/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Title,Id")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                    await _uniofWork.Category.CreateAsync(category);
                else
                    _uniofWork.Category.UpdateAsync(category);
                await _uniofWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _uniofWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            await _uniofWork.Category.DeleteJsAsync(id);
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
