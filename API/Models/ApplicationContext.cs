using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relation entre User et Cart
            modelBuilder.Entity<User>()
                .HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre User et Product
            modelBuilder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre Product et Cart
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Carts)
                .WithMany(c => c.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "CartProduct",
                    j => j.HasOne<Cart>().WithMany().HasForeignKey("CartId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasKey("ProductId", "CartId")
                );
        }
    }
}
