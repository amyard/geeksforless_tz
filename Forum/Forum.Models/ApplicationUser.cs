using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("Avatar")]
        public string ImageUrl { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
