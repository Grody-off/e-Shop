using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace e_Shop.Models
{
    public class AppDbContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;

        public AppDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().
                ToTable("Product").
                Property(p => p.Id).
                ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>().
                ToTable("Product").
                Property(p => p.Price).
                HasColumnType("decimal(5, 2)").
                IsRequired(true);

            modelBuilder.Entity<Cart>().
                ToTable("Cart").
                Property(c => c.CartId).
                ValueGeneratedOnAdd();
            
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}