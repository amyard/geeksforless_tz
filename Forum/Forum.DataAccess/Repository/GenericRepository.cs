﻿using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Specification;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public void UpdateAsync(T entity)
        {
            _context.Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<T> GetByIdAsyncWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetListAsyncWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }
    }
}
