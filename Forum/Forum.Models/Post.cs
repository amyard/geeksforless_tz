using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Post : BaseEntity
    {
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Body { get; set; }

        [DisplayName("Image")]
        public string ImageUrl { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; }


        [Key]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        // category
    }
}
