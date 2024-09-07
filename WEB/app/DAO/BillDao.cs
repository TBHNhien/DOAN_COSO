using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.DAO
{
    public class BillDao
    {
        private readonly ApplicationDbContext _context;

        public BillDao(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Order> ListOrderByUserId(string userId)
        {
            return _context.Orders.Where(x => x.UserId == userId).Include(d => d.OrderDetails).ThenInclude(p => p.Product).OrderByDescending(cd => cd.CreatedDate).ToList();
        }
    }
}
