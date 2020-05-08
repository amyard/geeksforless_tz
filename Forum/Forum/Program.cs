using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Forum.DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using Forum.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Forum.Utility;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Forum
{
    public class Program
    {
        /* override default Main for creating db in beginning if it's not exists */
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    // add migrations if db does not exists
                    context.Database.EnsureCreated();

                    // generate data must be here
                    await GenerateCategories(context);
                    await GenerateRolesAsync(roleManager, context);
                    await GenerateAdmin(userManager, context);
                    await GenerateModerator(userManager, context);
                    await GenerateUsers(userManager, context);
                    await context.SaveChangesAsync();

                    // save data
                    await GeneratePosts(context);
                    await context.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migrations.");
                }
            }

            host.Run();
        }

        private async static Task GeneratePosts(ApplicationDbContext context)
        {
            var brandsData = File.ReadAllText("../Forum.DataAccess/SeedData/posts_clean.json");
            var brands = JsonSerializer.Deserialize<List<Post>>(brandsData);

            foreach (var item in brands)
            {
                Post obj = new Post()
                {
                    Title = item.Title,
                    Body = item.Body,
                    CategoryId = item.CategoryId,
                    ImageUrl = item.ImageUrl,
                    ApplicationUserId = context.ApplicationUsers.OrderBy(c => Guid.NewGuid()).FirstOrDefault().Id
                };
                await context.Posts.AddAsync(obj);
            }
        }

        private static async Task GenerateUsers(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser{UserName = "user01@gmail.com", Email = "user01@gmail.com",
                                    FirstName = "Aval", LastName = "Symo", 
                                    ImageUrl = "img_seed/users/user1.png"},
                new ApplicationUser{UserName = "user02@gmail.com", Email = "user02@gmail.com",
                                    FirstName = "Nitro", LastName = "Agin", 
                                    ImageUrl = "img_seed/users/user2.jpg"},
                new ApplicationUser{UserName = "user03@gmail.com", Email = "user03@gmail.com",
                                    FirstName = "Trio", LastName = "Lamo", 
                                    ImageUrl = "img_seed/users/user3.jpg"},
                new ApplicationUser{UserName = "user04@gmail.com", Email = "user04@gmail.com",
                                    FirstName = "Anim", LastName = "Tiitri", 
                                    ImageUrl = "img_seed/users/user4.png"},
                new ApplicationUser{UserName = "user05@gmail.com", Email = "user05@gmail.com",
                                    FirstName = "Lilo", LastName = "Stitch",
                                    ImageUrl = "img_seed/users/user5.jpeg"},
                new ApplicationUser{UserName = "user06@gmail.com", Email = "user06@gmail.com",
                                    FirstName = "Alice", LastName = "Wonderland",
                                    ImageUrl = "img_seed/users/user1.png"},
                new ApplicationUser{UserName = "user07@gmail.com", Email = "user07@gmail.com",
                                    FirstName = "TC", LastName = "Junior", 
                                    ImageUrl = "img_seed/users/user2.jpg"},
                new ApplicationUser{UserName = "user08@gmail.com", Email = "user08@gmail.com",
                                    FirstName = "Argo", LastName = "Symo", 
                                    ImageUrl = "img_seed/users/user3.jpg"},
                new ApplicationUser{UserName = "user09@gmail.com", Email = "user09@gmail.com",
                                    FirstName = "Arkham", LastName = "Boggie", 
                                    ImageUrl = "img_seed/users/user4.png"},
                new ApplicationUser{UserName = "user10@gmail.com", Email = "user10@gmail.com",
                                    FirstName = "Lifro", LastName = "Alivo",
                                    ImageUrl = "img_seed/users/user5.jpeg"},
            };

            foreach (ApplicationUser user in users)
            {
                var result = await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, SD.Role_User);
            }
        }

        private static async Task GenerateModerator(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser{UserName = "moderator1@gmail.com", Email = "moderator1@gmail.com",
                                    FirstName = "Avalin", LastName = "ASymo",
                                    ImageUrl = "img_seed/users/moder1.jpg"},
                new ApplicationUser{UserName = "moderator2@gmail.com", Email = "moderator2@gmail.com",
                                    FirstName = "Aval", LastName = "Symo",
                                    ImageUrl = "img_seed/users/moder1.jpg"},
            };

            foreach(ApplicationUser user in users)
            {
                var result = await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, SD.Role_Moderator);
            }
        }

        private static async Task GenerateAdmin(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var user = new ApplicationUser
            {
                UserName = "delme@gmail.com",
                Email = "delme@gmail.com",
                FirstName = "delme",
                LastName = "Awesome",
                ImageUrl = "img_seed/users/admin.png"
            };

            var result = await userManager.CreateAsync(user, "Admin123*");
            await userManager.AddToRoleAsync(user, SD.Role_Admin);
        }

        public static async Task GenerateRolesAsync(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
            if (!await roleManager.RoleExistsAsync(SD.Role_Moderator))
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Moderator));
            if (!await roleManager.RoleExistsAsync(SD.Role_User))
                await roleManager.CreateAsync(new IdentityRole(SD.Role_User));
        }

        public static async Task GenerateCategories(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            { 
                var categories = new List<Category>
                {
                    new Category{Title = "Programming"},
                    new Category{Title = "Marketing"},
                    new Category{Title = "Business"},
                    new Category{Title = "Design"},
                    new Category{Title = "GameDevelop"}
                };
                categories.ForEach(s => context.Categories.AddAsync(s));
                await context.SaveChangesAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
