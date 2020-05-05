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
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    // add migrations if db does not exists
                    context.Database.EnsureCreated();

                    // generate data must be here
                    await GenerateCategories(context);
                    await GenerateRolesAsync(services, context);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migrations.");
                }
            }

            host.Run();
        }

        public static async Task GenerateRolesAsync(IServiceProvider services, ApplicationDbContext context)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
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
