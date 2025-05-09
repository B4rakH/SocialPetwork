using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Helper;
using System.Threading.Tasks;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using SocialNetworkForPets.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace SocialNetworkForPets
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Database Configuration
            string DbConnectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(DbConnectionString));

            //Configuration of services
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IHashtagService, HashtagService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IUsersService, UsersService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Authentication/Login";
                    options.AccessDeniedPath = "/Authentication/AccessDenied";
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            //Seed database with initial data
            using (var scope = app.Services.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                await DbInitializer.SeedAsync(dbContext);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
