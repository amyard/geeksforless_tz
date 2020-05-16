﻿using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        Task<int> CountAsync(ISpecification<T> spec);


        // clear post repo
        Post GetByIdAsyncWithComment(int id);
        void AddSubComment(SubComment comment);
        void DeleteAllCommentByPostId(int id);
    }
} 
