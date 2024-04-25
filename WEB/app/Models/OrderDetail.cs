using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class OrderDetail
    {
		public int Id { get; set; }
		public int OrderID { get; set; }


        public long ProductID { get; set; }

        
        public int Quantity { get; set; }


        public decimal Price { get; set; }

        [ValidateNever]
        public virtual Order Order { get; set; }

        [ValidateNever]
        public virtual Product Product { get; set; }
    }
}
