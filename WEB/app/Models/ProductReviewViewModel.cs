namespace app.Models
{
	public class ProductReviewViewModel
	{
		public int ReviewId { get; set; }
		public long ProductId { get; set; } // Giả sử bạn muốn hiển thị ID sản phẩm trong đánh giá
		public string UserId { get; set; }
		public string UserName { get; set; } // Thêm tên người dùng ở đây
		public decimal Rating { get; set; }
		public string ReviewText { get; set; }
		public DateTime ReviewDate { get; set; }

		// Nếu bạn muốn thêm tham chiếu đến sản phẩm và người dùng, bạn cũng có thể thêm:
		// public Product Product { get; set; }
		// public IdentityUser User { get; set; }
	}

}
