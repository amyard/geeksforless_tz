using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Forum.DataAccess.Repository.IRepository
{
    public interface IFileManager
    {
        Task<string> SaveImage(IFormFileCollection files, string imageBasePath, string imageResultPath);
        bool RemoveImage(string filePath);
    }
}
