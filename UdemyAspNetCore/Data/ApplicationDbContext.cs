using Microsoft.EntityFrameworkCore;
using UdemyAspNetCore.Models;

namespace UdemyAspNetCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Category> Category { get; set; }
        public DbSet <ApplicationType> ApplicationType { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
