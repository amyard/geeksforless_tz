﻿using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository.IRepository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetListAsync();

        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        Task<T> DeleteJsAsync(int id);
        Task<bool> SaveChangesAsync();


        // repo with specification
        Task<T> GetByIdAsyncWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> GetListAsyncWithSpec(ISpecification<T> spec);


        // TODO - override as UnitOfWork for better structure
        // clear post repo
        Post GetByIdAsyncWithComment(int id);
        void AddSubComment(SubComment comment);
    }
}
