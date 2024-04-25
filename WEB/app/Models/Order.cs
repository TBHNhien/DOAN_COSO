using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Order
    {
        public int ID { get; set; }
        
        //[ForeignKey("User")]
        public string UserId { get; set; }

		public DateTime CreatedDate { get; set; }

		public decimal TotalPrice { get; set; }

		public string? Notes { get; set; }
		public string? ShipName { get; set; }
        public string? ShipMobile { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipEmail { get; set; }
        public int? Status { get; set; }

		public IdentityUser User { get; set; }

		public List<OrderDetail> OrderDetails { get; set; }

		// Khai báo thuộc tính navigation để tham chiếu đến User
		//public virtual User User { get; set; }
	}
}
