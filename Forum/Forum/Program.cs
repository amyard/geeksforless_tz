using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Forum.DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using Forum.Models;
using System.Collections.Generic;
using System.Linq;

namespace Forum
{
    public class Program
    {
        /* override default Main for creating db in beginning if it's not exists */
        public static void Main(string[] args)
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
                    GenerateCategories(context);
                    GenerateRoles(context);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migrations.");
                }
            }

            host.Run();
        }

        private static void GenerateCategories(ApplicationDbContext context)
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
                categories.ForEach(s => context.Categories.AddAsync(s).GetAwaiter().GetResult());
                context.SaveChanges();
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
