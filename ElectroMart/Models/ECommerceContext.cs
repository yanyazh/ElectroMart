using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace myStore.Models
{
    public class ECommerceContext : IdentityDbContext<IdentityUser>
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<News> NewsArticles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity

            // Set up the relationship between Product and Category
            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Category)
                        .WithMany(c => c.Products)
                        .HasForeignKey(p => p.CategoryId)
                        .OnDelete(DeleteBehavior.SetNull); // Optional: define delete behavior
        }
    }
}
