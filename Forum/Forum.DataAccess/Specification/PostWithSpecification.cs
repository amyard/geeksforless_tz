using Forum.Models;

namespace Forum.DataAccess.Specification
{
    public class PostWithSpecification : BaseSpecification<Post>
    {
        // for list view
        public PostWithSpecification(PostSpecParams postParams)
            : base(x => 
                (string.IsNullOrEmpty(postParams.Search) || x.Title.ToLower().Contains(postParams.Search)) && 
                (!postParams.CategoryId.HasValue || x.CategoryId == postParams.CategoryId)
            )
        {
            AddInclude(x => x.ApplicationUser);
            AddInclude(x => x.Category);
            ApplyPaging(postParams.PageSize * (postParams.PageIndex - 1), postParams.PageSize);
        }

        // for detail view
        public PostWithSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.ApplicationUser);
            AddInclude(x => x.Category);
        }
    }
}
