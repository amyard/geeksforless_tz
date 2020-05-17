﻿using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Specification;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public void UpdateAsync(Post post)
        {
            var objFromDb = _context.Posts.Find(post.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = post.Title;
                objFromDb.Body = post.Body;
                objFromDb.CategoryId = post.CategoryId;
                objFromDb.ImageUrl = post.ImageUrl;
                objFromDb.Modified = DateTime.Now;
            }
        }

        public async Task<T> GetByIdAsyncWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetListAsyncWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }



        // clear Posts repo
        public Post GetByIdAsyncWithComment(int id)
        {
            return _context.Posts
                .Include(p => p.Category)
                .Include(p => p.ApplicationUser)
                .Include(p => p.MainComments)
                    .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void DeleteAllCommentByPostId(int id)
        {
            var mainComments = _context.Posts.Where(c => c.Id == id).Select(o => o.MainComments).ToList();

            foreach (var comm in mainComments)
            {
                _context.RemoveRange(comm);
            }
            _context.SaveChanges();
        }

        public Post GetPostByMainCommentId(int id)
        {
            return _context.Posts.Where(s => s.MainComments.Select(m => m.Id).Contains(id)).FirstOrDefault();
        }
    }
}
