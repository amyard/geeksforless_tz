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
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Forum.Models.Comments;
using Forum.DataAccess;

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
                var webHost = services.GetRequiredService<IWebHostEnvironment>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    // add migrations if db does not exists
                    context.Database.EnsureCreated();

                    // generate data must be here
                    await GenerateCategories(context);
                    await GenerateRolesAsync(roleManager);
                    await GenerateNewUsers(userManager, context);
                    await context.SaveChangesAsync();

                    // save data
                    await GeneratePosts(context);
                    await context.SaveChangesAsync();

                    // save data
                    await GenerateMainComment(context);
                    await context.SaveChangesAsync();

                    await GenerateSubComment(context);
                    await context.SaveChangesAsync();

                    // generate folders
                    string webRootPath = webHost.WebRootPath;
                    var postsPath = Path.Combine(webRootPath, SD.Post_Image_Base_Path.TrimStart('\\'));
                    if (!Directory.Exists(postsPath))
                        Directory.CreateDirectory(postsPath);

                    var usersPath = Path.Combine(webRootPath, SD.Users_Image_Base_Path.TrimStart('\\'));
                    if (!Directory.Exists(usersPath))
                        Directory.CreateDirectory(usersPath);

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migrations.");
                }
            }

            host.Run();
        }


        private async static Task GenerateSubComment(ApplicationDbContext context)
        {
            if (!context.SubComments.Any())
            {
                var commentData = File.ReadAllText("../Forum.DataAccess/SeedData/maincomment.json");
                var comm = JsonSerializer.Deserialize<List<MainComment>>(commentData);

                foreach (var item in comm)
                {
                    var comment = new SubComment
                    {
                        MainCommentId = context.MainComments.OrderBy(c => Guid.NewGuid()).FirstOrDefault().Id,
                        Message = item.Message,
                        Created = DateTime.Now,
                        ApplicationUserId = context.ApplicationUsers.OrderBy(c => Guid.NewGuid()).FirstOrDefault().Id,
                    };
                    await context.SubComments.AddAsync(comment);
                }
                await context.SaveChangesAsync();
            }
        }

        private async static Task GenerateMainComment(ApplicationDbContext context)
        {
            if (!context.MainComments.Any())
            {
                var commentData = File.ReadAllText("../Forum.DataAccess/SeedData/maincomment.json");
                var comm = JsonSerializer.Deserialize<List<MainComment>>(commentData);

                foreach (var item in comm)
                {
                    var post = context.Posts.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
                    post.MainComments = post.MainComments ?? new List<MainComment>();

                    post.MainComments.Add(new MainComment
                    {
                        Message = item.Message,
                        Created = DateTime.Now,
                        ApplicationUserId = context.ApplicationUsers.OrderBy(c => Guid.NewGuid()).FirstOrDefault().Id
                    });

                    context.Posts.Update(post);
                }
                await context.SaveChangesAsync();
            }
        }

        private async static Task GeneratePosts(ApplicationDbContext context)
        {
            if (!context.Posts.Any())
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
        }


        private static async Task GenerateNewUsers(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            if(!context.ApplicationUsers.Any())
            {
                // Generate Admin Role
                var user = new ApplicationUser
                {
                    UserName = "delme@gmail.com",
                    Email = "delme@gmail.com",
                    FirstName = "delme",
                    LastName = "Awesome",
                    ImageUrl = "img_seed/users/admin.png",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, SD.Role_Admin);

                // Generate Moderator
                var moderators = new List<ApplicationUser>
                {
                    new ApplicationUser{UserName = "moderator1@gmail.com", Email = "moderator1@gmail.com",
                                        FirstName = "Avalin", LastName = "ASymo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/moder1.jpg"},
                    new ApplicationUser{UserName = "moderator2@gmail.com", Email = "moderator2@gmail.com",
                                        FirstName = "Aval", LastName = "Symo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/moder1.jpg"},
                };

                foreach (ApplicationUser moder in moderators)
                {
                    await userManager.CreateAsync(moder, "Admin123*");
                    await userManager.AddToRoleAsync(moder, SD.Role_Moderator);
                }

                // Generate users
                var users = new List<ApplicationUser>
                {
                    new ApplicationUser{UserName = "user01@gmail.com", Email = "user01@gmail.com",
                                        FirstName = "Aval", LastName = "Symo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user1.png"},
                    new ApplicationUser{UserName = "user02@gmail.com", Email = "user02@gmail.com",
                                        FirstName = "Nitro", LastName = "Agin", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user2.jpg"},
                    new ApplicationUser{UserName = "user03@gmail.com", Email = "user03@gmail.com",
                                        FirstName = "Trio", LastName = "Lamo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user3.jpg"},
                    new ApplicationUser{UserName = "user04@gmail.com", Email = "user04@gmail.com",
                                        FirstName = "Anim", LastName = "Tiitri", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user4.png"},
                    new ApplicationUser{UserName = "user05@gmail.com", Email = "user05@gmail.com",
                                        FirstName = "Lilo", LastName = "Stitch", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user5.jpeg"},
                    new ApplicationUser{UserName = "user06@gmail.com", Email = "user06@gmail.com",
                                        FirstName = "Alice", LastName = "Wonderland", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user1.png"},
                    new ApplicationUser{UserName = "user07@gmail.com", Email = "user07@gmail.com",
                                        FirstName = "TC", LastName = "Junior", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user2.jpg"},
                    new ApplicationUser{UserName = "user08@gmail.com", Email = "user08@gmail.com",
                                        FirstName = "Argo", LastName = "Symo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user3.jpg"},
                    new ApplicationUser{UserName = "user09@gmail.com", Email = "user09@gmail.com",
                                        FirstName = "Arkham", LastName = "Boggie", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user4.png"},
                    new ApplicationUser{UserName = "user10@gmail.com", Email = "user10@gmail.com",
                                        FirstName = "Lifro", LastName = "Alivo", EmailConfirmed = true,
                                        ImageUrl = "img_seed/users/user5.jpeg"},
                };

                foreach (ApplicationUser usr in users)
                {
                    await userManager.CreateAsync(usr, "Admin123*");
                    await userManager.AddToRoleAsync(usr, SD.Role_User);
                }
            }  
        }

        public static async Task GenerateRolesAsync(RoleManager<IdentityRole> roleManager)
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
