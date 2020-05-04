using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Category Name")]
        [MaxLength(80)]
        public string Title { get; set; }
    }
}
