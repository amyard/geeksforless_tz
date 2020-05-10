using System.ComponentModel.DataAnnotations;

namespace Forum.Models.ViewModels
{
    public class CommentVM
    {
        [Required]
        public int PostId { set; get; }
        [Required]
        public int MainCommentId { set; get; }
        [Required]
        public string Message { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
