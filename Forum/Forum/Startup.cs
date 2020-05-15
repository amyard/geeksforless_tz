using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Forum.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Forum.DataAccess.Repository.IRepository;
using Forum.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using Forum.DataAccess.Services;

namespace Forum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // repository general
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // save images
            services.AddTransient<IFileManager, FileManager>();


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            // override for using Roles
            //services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // email send - EmailSender was created in Forum.Utility -> Services
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // override loginrequiredmixin
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // facebook and google authentication
            services.AddAuthentication().AddFacebook(options => {
                options.AppId = Configuration.GetValue<string>("OAuth:Facebook:Id");
                options.AppSecret = Configuration.GetValue<string>("OAuth:Facebook:Secret");
            });
            services.AddAuthentication().AddGoogle(options => {
                options.ClientId = Configuration.GetValue<string>("OAuth:Google:Id");
                options.ClientSecret = Configuration.GetValue<string>("OAuth:Google:Secret");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Forum}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
