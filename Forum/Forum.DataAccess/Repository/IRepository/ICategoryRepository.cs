using Forum.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetListAsync();
        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        Task DeleteJsAsync(int id);
        IEnumerable<SelectListItem> GetSelectListAsync();
    }
}
