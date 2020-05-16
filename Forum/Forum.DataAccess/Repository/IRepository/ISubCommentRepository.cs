using Forum.Models;
using Forum.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository.IRepository
{
    public interface ISubCommentRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<T> DeleteComment(int id);
        Task AddCommentFromCommentView(CommentVM vm, Claim claim);
    }
}
