using System;

namespace Forum.Models.Comments
{
    public class Comment : BaseEntity
    {
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
