using Forum.Models;

namespace Forum.DataAccess.Specification
{
    public class PostWithFiltersForCountSpecification : BaseSpecification<Post>
    {
        public PostWithFiltersForCountSpecification(PostSpecParams postParams)
            : base(x =>
                (!postParams.CategoryId.HasValue || x.CategoryId == postParams.CategoryId)
            )
        {
        }
    }
}
