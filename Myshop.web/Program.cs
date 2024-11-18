using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using myshop.Utilities;
using Myshop.DataAccess.Data;
using Myshop.DataAccess.Implementation;
using Myshop.Entities.Repositories;


namespace TaskITIMvc2024
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //add connection string to project 
			builder.Services.AddDbContext<ApplicationDbContext>
                (options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                });

            builder.Services.AddIdentity<IdentityUser,IdentityRole>(options=>
            options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromDays(4))
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            //must register repository to project resolve service
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


			// Add services to the container.
			builder.Services.AddControllersWithViews();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
               name: "Admin",
               pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
