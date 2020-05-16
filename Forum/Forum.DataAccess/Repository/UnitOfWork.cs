using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using Forum.Models.Comments;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db,
            ICategoryRepository<Category> _category,
            IMainCommentRepository<MainComment> _mainComment,
            ISubCommentRepository<SubComment> _subComment,
            IGenericRepository<Post> _post)
        {
            _db = db;
            Category = _category;
            MainComment = _mainComment;
            SubComment = _subComment;
            Post = _post;
        }

        public ICategoryRepository<Category> Category { get; set; }
        public IMainCommentRepository<MainComment> MainComment { get; set; }
        public ISubCommentRepository<SubComment> SubComment { get; set; }
        public IGenericRepository<Post> Post { get; set; }


        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _db.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
