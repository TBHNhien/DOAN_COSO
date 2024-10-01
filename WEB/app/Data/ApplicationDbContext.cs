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

        public DbSet<FavouriteProduct> FavouriteProducts { get; set; }

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//    base.OnModelCreating(modelBuilder); // Don't forget to call this!

		//    // Configure the primary key for OrderDetail
		//    modelBuilder.Entity<OrderDetail>()
		//        .HasKey(od => new { od.OrderID, od.ProductID });

		//    // Other Fluent API configurations
		//}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Cấu hình kiểu cột cho các thuộc tính decimal
			modelBuilder.Entity<Order>()
				.Property(o => o.TotalPrice)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<OrderDetail>()
				.Property(od => od.Price)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Product>()
				.Property(p => p.PromotionPrice)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<ProductReview>()
				.Property(pr => pr.Rating)
				.HasColumnType("decimal(18,2)");

			base.OnModelCreating(modelBuilder);
		}

	}
}
