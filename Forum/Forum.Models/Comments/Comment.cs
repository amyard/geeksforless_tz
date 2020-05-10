using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models.Comments
{
    public class Comment : BaseEntity
    {
        public string Message { get; set; }
        public DateTime Created { get; set; }

        [Column("CommentUserId")]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
