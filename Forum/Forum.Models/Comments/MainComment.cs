using System.Collections.Generic;

namespace Forum.Models.Comments
{
    public class MainComment : Comment
    {
        public List<SubComment> SubComments { set; get; }
    }
}
