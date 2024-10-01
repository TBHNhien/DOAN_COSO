using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class FavouriteProduct
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public long ProductId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; }
        public IdentityUser User { get; set; }
    }
}
