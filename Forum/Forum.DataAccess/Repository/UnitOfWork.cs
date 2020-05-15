using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        // private readonly IGenericRepository<Category> category;
        public UnitOfWork(ApplicationDbContext db,
            IGenericRepository<Category> _category)
        {
            _db = db;
            Category = _category;
        }

        public IGenericRepository<Category> Category { get; set; }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
