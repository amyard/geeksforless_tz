using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using System;

namespace Forum.DataAccess.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Category { get; }
        IGenericRepository<Post> Post { get; }
    }
}
