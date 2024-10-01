using app.Data;

namespace app.DAO
{
	public class FavouriteProductDao
	{
		private readonly ApplicationDbContext _context;

		public FavouriteProductDao(ApplicationDbContext context)
		{
			_context = context;
		}
		public int CheckFavouriteProduct(String userId, long productId)
		{
			if(_context.FavouriteProducts.Where(x => x.UserId == userId && x.ProductId == productId).Any()) {
				return 1;
			}
			return 0;
		}
	}
}
