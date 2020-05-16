using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using Forum.Models.Comments;
using System;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository<Category> Category { get; }
        IMainCommentRepository<MainComment> MainComment { get; }
        ISubCommentRepository<SubComment> SubComment { get; }
        IGenericRepository<Post> Post { get; }
        Task<bool> SaveChangesAsync();
    }
}
