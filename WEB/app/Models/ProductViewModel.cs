﻿namespace app.Models
{
	public class ProductViewModel
	{
		public List<Product> Products { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }

		public string SearchQuery { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

		//public int? CategoryId { get; set; } // Thêm thuộc tính CategoryId
	}
}
