using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class CommentRepository<T> : ICommentRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task<T> DeleteJsAsync(int id)
        {
            var objData = await _context.Set<T>().FindAsync(id);
            _context.Remove(objData);
            await _context.SaveChangesAsync();
            return objData;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetListAsync()
        {
            return await _context.Set<T>().OrderByDescending(q => q.Id).ToListAsync();
        }

        public void UpdateAsync(T entity)
        {
            _context.Update(entity);
        }
    }
}
