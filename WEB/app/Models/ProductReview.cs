using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
	public class ProductReview
	{
		[Key]
		public int ReviewId { get; set; }

		[ForeignKey("Product")]
		public long ProductId { get; set; }

		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

		[Range(0, 5)]
		public decimal Rating { get; set; }

		public string ReviewText { get; set; }

		public DateTime ReviewDate { get; set; } = DateTime.Now;

		public virtual Product Product { get; set; }
		public IdentityUser User { get; set; }
	}
}
