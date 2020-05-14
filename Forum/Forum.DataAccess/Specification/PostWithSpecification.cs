using Forum.Models;

namespace Forum.DataAccess.Specification
{
    public class PostWithSpecification : BaseSpecification<Post>
    {
        // for list view
        public PostWithSpecification(int? categoryId)
            : base(x => 
                (!categoryId.HasValue || x.CategoryId == categoryId)
            )
        {
            AddInclude(x => x.ApplicationUser);
            AddInclude(x => x.Category);
        }

        // for detail view
        public PostWithSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.ApplicationUser);
            AddInclude(x => x.Category);
        }
    }
}
