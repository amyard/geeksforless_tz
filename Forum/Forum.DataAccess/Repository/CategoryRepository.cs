using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class CategoryRepository<T> : ICategoryRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task DeleteJsAsync(int id)
        {
            var objData = await _context.Set<T>().FindAsync(id);
            _context.Remove(objData);
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetListAsync()
        {
            return await _context.Set<T>().OrderByDescending(q => q.Id).ToListAsync();
        }

        public IEnumerable<SelectListItem> GetSelectListAsync()
        {
            return _context.Categories.ToList().Select(i => new SelectListItem
            {
                Text = i.Title,
                Value = i.Id.ToString()
            });
        }

        public void UpdateAsync(T entity)
        {
            _context.Update(entity);
        }
    }
}
