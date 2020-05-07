using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }

        [DisplayName("Avatar")]
        public string ImageUrl { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
