using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Myshop.Entities.Models;

namespace Myshop.DataAccess.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {   
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           base.OnModelCreating(builder);
            builder.Entity<IdentityUser>()
            .Property("Discriminator")
            .HasDefaultValue("IdentityUser");

            builder.Entity<IdentityUser>()
          .HasDiscriminator<string>("Discriminator")
          .HasValue<IdentityUser>("IdentityUser");
        }
        

    }
}
