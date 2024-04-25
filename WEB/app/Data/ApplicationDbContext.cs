using app.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace app.Data
{
    public class ApplicationDbContext : IdentityDbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }




        public DbSet<Content> Contents { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Slide> Slides { get; set; }
        //public virtual DbSet<Content> Contents { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

		public DbSet<ProductReview> ProductReviews { get; set; }

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//    base.OnModelCreating(modelBuilder); // Don't forget to call this!

		//    // Configure the primary key for OrderDetail
		//    modelBuilder.Entity<OrderDetail>()
		//        .HasKey(od => new { od.OrderID, od.ProductID });

		//    // Other Fluent API configurations
		//}

	}
}
