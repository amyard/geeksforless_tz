using Forum.Models;
using Forum.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Forum.DataAccess.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationDbContextSeed(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // create custom data if not exists
        public static async Task SeedAsync(ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.Categories.Any())
                {
                    var categoryData = File.ReadAllText("../Forum.DataAccess/Data/DataSeed/categories.json");
                    var category = JsonSerializer.Deserialize<List<Category>>(categoryData);
                    foreach (var item in category)
                    {
                        context.Categories.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                // create roles, if not exists
                //if (await _roleManager.RoleExistsAsync(SD.Role_Admin))
                //    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                //if (!await _roleManager.RoleExistsAsync(SD.Role_Moderator))
                //    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Moderator));
                //if (!await _roleManager.RoleExistsAsync(SD.Role_User))
                //    await _roleManager.CreateAsync(new IdentityRole(SD.Role_User));
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}
