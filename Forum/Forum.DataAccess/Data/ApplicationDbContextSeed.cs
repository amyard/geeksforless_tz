using Forum.Models;
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
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}
