using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Forum.DataAccess;
using Forum.DataAccess.Data;
using Forum.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Forum.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IFileManager _fileManager;     // for upload images on server

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IFileManager fileManager)
        {
            _fileManager = fileManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string ImageUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            // custom added additional data
            [Required]
            [Display(Name = "First Name")]
            [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Last Name")]
            [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
            public string LastName { get; set; }
            public string ImageUrl { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = _context.ApplicationUsers.Where(a=>a.Email == user.Email).ToList().FirstOrDefault().FirstName;
            var lastName = _context.ApplicationUsers.Where(a => a.Email == user.Email).ToList().FirstOrDefault().LastName;
            var imageUrl = _context.ApplicationUsers.Where(a => a.Email == user.Email).ToList().FirstOrDefault().ImageUrl;

            Username = userName;
            ImageUrl = imageUrl;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            // save new user data
            var obj = _context.ApplicationUsers.Where(a => a.Email == user.Email).ToList().FirstOrDefault();
            if(obj.Id != null)
            { 
                obj.FirstName = Input.FirstName;
                obj.LastName = Input.LastName;
                obj.PhoneNumber = Input.PhoneNumber;

                // SAVE IMAGE
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    // DELETE OLD IMAGE
                    _fileManager.RemoveImage(obj.ImageUrl);
                    obj.ImageUrl = await _fileManager.SaveImage(files, SD.Users_Image_Base_Path, SD.Users_Image_Result_Path);
                }
                await _context.SaveChangesAsync();
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
