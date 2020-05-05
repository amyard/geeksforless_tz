using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Post : BaseEntity
    {
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Body { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string ImageUrl { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        // author , category
    }
}
