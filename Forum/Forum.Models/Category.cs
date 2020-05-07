using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Category Name")]
        [StringLength(80, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; }
    }
}
