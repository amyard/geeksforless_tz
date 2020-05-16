using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;

namespace Forum.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db,
            IGenericRepository<Category> _category,
            IGenericRepository<Post> _post)
        {
            _db = db;
            Category = _category;
            Post = _post;
        }

        public IGenericRepository<Category> Category { get; set; }
        public IGenericRepository<Post> Post { get; set; }


        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
