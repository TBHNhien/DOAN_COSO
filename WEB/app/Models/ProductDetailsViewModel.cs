namespace app.Models
{
	public class ProductDetailsViewModel
	{
		public Product Product { get; set; }
		public ProductReview NewReview { get; set; }
		//public IEnumerable<ProductReview> ProductReview { get; set; }

		public List<ProductReviewViewModel> ProductReviews { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }



		public ProductCategory Category { get; set; }
	}

}
