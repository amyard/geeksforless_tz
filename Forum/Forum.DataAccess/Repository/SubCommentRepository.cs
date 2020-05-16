using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class SubCommentRepository<T> : ISubCommentRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        public SubCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> DeleteComment(int id)
        {
            var obj = await _context.Set<T>().FindAsync(id);
            _context.Remove(obj);
            return obj;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
