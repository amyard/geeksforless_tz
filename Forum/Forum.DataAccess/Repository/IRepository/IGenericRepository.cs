﻿using Forum.DataAccess.Specification;
using Forum.Models;
using Forum.Models.Comments;
using Forum.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository.IRepository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetListAsync();

        Task CreateAsync(T entity);
        void UpdateAsync(Post post);
        Task DeleteJsAsync(int id);


        // repo with specification
        Task<T> GetByIdAsyncWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> GetListAsyncWithSpec(ISpecification<T> spec);
        Task<int> CountAsync(ISpecification<T> spec);


        // clear post repo
        Post GetByIdAsyncWithComment(int id);
        void DeleteAllCommentByPostId(int id);

        Post GetPostByMainCommentId(int id);
    }
} 
