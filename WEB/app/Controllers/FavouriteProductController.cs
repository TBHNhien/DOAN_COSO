using app.Data;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Dao;

namespace app.Controllers
{
	public class FavouriteProductController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		public FavouriteProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		[Authorize]
		public IActionResult Index(int page = 1)
		{
			var temp = _context.FavouriteProducts.Where(x => x.UserId == _userManager.GetUserId(User)).Include("Product").ToList();
			const int pageSize = 9;
			int totalProductCount;
			int totalPages;
			ProductViewModel viewModel;
			List<Product> products = new List<Product>();
			foreach (var item in temp)
			{
				products.Add(item.Product);
			}
			products = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
			totalProductCount = products.Count();
			totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

			viewModel = new ProductViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = totalPages
			};

			return View(viewModel);
			
		}
		[Authorize]
		[HttpGet]
		public IActionResult Add(long id)
		{
			var userId = _userManager.GetUserId(User);
			var product = _context.Products.Find(id);
			
			if(_context.FavouriteProducts.Where(x => x.ProductId == id && x.UserId == userId).Any()) {
				_context.FavouriteProducts.Remove(_context.FavouriteProducts.Where(x => x.ProductId == id && x.UserId == userId).FirstOrDefault());
				_context.SaveChanges();
				return Redirect("/chi-tiet/" + product.MetaTitle + "/" + product.Id.ToString());
			}
			FavouriteProduct temp = new FavouriteProduct
			{
				ProductId = id,
				UserId = userId,

			};
			_context.FavouriteProducts.Add(temp);
			_context.SaveChanges();
			return Redirect("/chi-tiet/" + product.MetaTitle + "/" + product.Id.ToString());
		}
	}
}
