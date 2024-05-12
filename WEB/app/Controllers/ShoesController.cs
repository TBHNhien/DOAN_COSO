using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Model.Dao;
using System.Security.Claims;

namespace app.Controllers
{
	public class ShoesController : Controller
	{
		private const string CartSession = "CartSession";
		private readonly ProductDao _productDao;
		private readonly ProductCategoryDao _productCategoryDao;
		private readonly UserManager<IdentityUser> _userManager;

		public ShoesController(ProductDao productDao, ProductCategoryDao productCategoryDao, UserManager<IdentityUser> userManager)
		{
			_productDao = productDao;
			_productCategoryDao = productCategoryDao;
			_userManager = userManager;
		}

		public IActionResult Index()
		{

			return View();
		}

		//public IActionResult Shop()
		//{
		//	ViewBag.Product = _productDao.ListProduct();
		//	return View();
		//}

		//Phân trang
		public IActionResult Shop(int page = 1)
		{


			const int pageSize = 9;
			var products = _productDao.ListProduct()
									  .Skip((page - 1) * pageSize)
									  .Take(pageSize)
									  .ToList();
			var totalProductCount = _productDao.ListProduct().Count();
			var totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

			var viewModel = new ProductViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = totalPages
			};

			return View(viewModel);
		}




		//public IActionResult Detail(long id)
		//{
		//	var product = _productDao.ViewDetail(id);
		//	ViewBag.Category = _productCategoryDao.ViewDetail(product.CategoryId.Value);
		//	//ViewBag.RelatedProducts = _productDao.ListRelatedProduct(id);
		//	return View(product);
		//}

		public async Task<IActionResult> Detail(long id,int page = 1)
		{
			const int pageSize = 2; // Đặt số lượng đánh giá trên mỗi trang là 2

			var product = _productDao.ViewDetail(id);
			var category = _productCategoryDao.ViewDetail(product.CategoryId.Value);
			var reviewsQuery = _productDao.ListReviewsByProductId(id); 

			// Lấy tổng số lượng đánh giá
			int reviewCount = reviewsQuery.Count();

			// Lấy đánh giá theo trang
			var pagedReviewsQuery = reviewsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
			var reviewUserIds = pagedReviewsQuery.Select(r => r.UserId).Distinct().ToList();
			var userNames = await _userManager.Users
									  .Where(u => reviewUserIds.Contains(u.Id))
									  .ToDictionaryAsync(u => u.Id, u => u.UserName);

			var pagedReviews = pagedReviewsQuery.Select(review => new ProductReviewViewModel
			{
				// ... cài đặt các thuộc tính của đánh giá...
				ReviewId = review.ReviewId,
				ProductId = review.ProductId,

				UserId = review.UserId,
				UserName = userNames[review.UserId],
				Rating = review.Rating,
				ReviewText = review.ReviewText,
				ReviewDate = review.ReviewDate
			}).ToList();

			var viewModel = new ProductDetailsViewModel
			{
				Product = product,
				Category = category,
				ProductReviews = pagedReviews,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling(reviewCount / (double)pageSize)
			};

			return View(viewModel);
		}



		[Authorize]
		[HttpPost]
		public IActionResult Detail(long id, int rating, string reviewText)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


			var review = new ProductReview
			{
				ProductId = id,
				UserId = userId, // Đảm bảo rằng bạn đang thiết lập UserId này
				Rating = rating,
				ReviewText = reviewText,
				ReviewDate = DateTime.Now
			};

			// Gọi phương thức AddReview để thêm đánh giá vào cơ sở dữ liệu
			_productDao.AddReview(review);

			// Chuyển hướng người dùng trở lại trang chi tiết sản phẩm
			return RedirectToAction("Detail", new { id = id });
		}

	}
		//public IActionResult AddItem(long productId,int quantity)
		//{
		//	var session = Session[CartSession];
		//}

}
