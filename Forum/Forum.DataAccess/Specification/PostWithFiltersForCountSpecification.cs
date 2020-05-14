using Forum.Models;

namespace Forum.DataAccess.Specification
{
    public class PostWithFiltersForCountSpecification : BaseSpecification<Post>
    {
        public PostWithFiltersForCountSpecification(PostSpecParams postParams)
            : base(x =>
                (string.IsNullOrEmpty(postParams.Search) || x.Title.ToLower().Contains(postParams.Search)) &&
                (!postParams.CategoryId.HasValue || x.CategoryId == postParams.CategoryId)
            )
        {
        }
    }
}
