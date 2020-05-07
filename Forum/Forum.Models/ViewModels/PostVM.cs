using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Forum.Models.ViewModels
{
    public class PostVM
    {
        public Post Post { get; set; }

        // for making dropdown menu for selecting category
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
