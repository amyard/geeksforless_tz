using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Forum.Models;
using Forum.Models.Comments;
using Forum.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository
{
    public class MainCommentRepository<T> : IMainCommentRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        public MainCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCommentFromCommentView(CommentVM vm, Claim claim)
        {
            var post = _context.Posts.Find(vm.PostId);
            post.MainComments = post.MainComments ?? new List<MainComment>();
            post.MainComments.Add(new MainComment
            {
                Message = vm.Message,
                Created = DateTime.Now,
                ApplicationUserId = claim.Value,
            });
            _context.Posts.Update(post);
        }

        public async Task DeleteMainAndSubComments(int id)
        {
            //var allSub = _db.SubComments.ToList();
            //allSub.RemoveAll(s => s.MainCommentId == id);

            var comment = await _context.Set<T>().FindAsync(id);
            var subComments = _context.SubComments.Where(s => comment.Id == id).ToList();
            _context.SubComments.RemoveRange(subComments);
            _context.Remove(comment);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
