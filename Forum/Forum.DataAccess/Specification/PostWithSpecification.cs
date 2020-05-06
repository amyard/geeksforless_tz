using Forum.Models;

namespace Forum.DataAccess.Specification
{
    public class PostWithSpecification : BaseSpecification<Post>
    {
        // for list view
        public PostWithSpecification()
        {
            AddInclude(x => x.ApplicationUser);
            //AddInclude(x => x.Category);
        }

        // for detail view
        public PostWithSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.ApplicationUser);
            //AddInclude(x => x.Category);
        }
    }
}
