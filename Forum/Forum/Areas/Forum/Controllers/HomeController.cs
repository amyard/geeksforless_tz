using System.Diagnostics;
using System.Threading.Tasks;
using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.ViewModels;
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
        public async Task<IActionResult> Details(int id)
        {
            // using Specification
            var spec = new PostWithSpecification(id);
            var post = await _context.GetByIdAsyncWithSpec(spec);
            if (post == null)
                return NotFound();
            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
