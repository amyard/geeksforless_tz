using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using System;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository<Category> Category { get; }
        IGenericRepository<Post> Post { get; }
        Task<bool> SaveChangesAsync();
    }
}
