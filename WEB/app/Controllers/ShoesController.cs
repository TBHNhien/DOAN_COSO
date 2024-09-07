using app.DAO;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Model.Dao;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Claims;

namespace app.Controllers
{
	public class ShoesController : Controller
	{
		private const string CartSession = "CartSession";
		private readonly ProductDao _productDao;
		private readonly ProductCategoryDao _productCategoryDao;
		private readonly FavouriteProductDao _favouriteProductDao;
		private readonly UserManager<IdentityUser> _userManager;

		public ShoesController(ProductDao productDao, ProductCategoryDao productCategoryDao, UserManager<IdentityUser> userManager, FavouriteProductDao favouriteProductDao)
		{
			_productDao = productDao;
			_productCategoryDao = productCategoryDao;
			_userManager = userManager;
			_favouriteProductDao = favouriteProductDao;
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





		public IActionResult Shop(int page = 1, int? categoryId = null)
		{
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			string reviews = _productDao.GetReviews(currentUserId);
			const int pageSize = 9;
			int totalProductCount = 0; // Khởi tạo với giá trị mặc định
			int totalPages;
			ProductViewModel viewModel;

			var productCategories = _productCategoryDao.ListAll().ToList(); // Lấy danh sách danh mục sản phẩm

			List<Product> products;

			if (categoryId.HasValue)
			{
				products = _productDao.ListByCategoryId(categoryId.Value, ref totalProductCount, page, pageSize);
			}
			else
			{
				products = _productDao.ListProduct().Skip((page - 1) * pageSize).Take(pageSize).ToList();
				totalProductCount = _productDao.ListProduct().Count();
			}

			totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

			viewModel = new ProductViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = totalPages,
				ProductCategories = productCategories, // Gán danh sách danh mục sản phẩm
				SearchQuery = categoryId.HasValue ? productCategories.FirstOrDefault(c => c.Id == categoryId.Value)?.Name : null
			};

			ViewBag.CategoryId = categoryId;

			if (!User.Identity.IsAuthenticated || reviews == "[]")
			{
				return View(viewModel);
			}

			// Gọi tới script Python để tính toán
			var recommendation = RunPythonScript("recommendation_script.py", reviews);

			string[] numbers = recommendation.Split(',');
			// Convert string array to list of integers
			List<long> numberList = new List<long>();
			foreach (string number in numbers)
			{
				if (long.TryParse(number, out long result))
				{
					numberList.Add(result);
				}
			}
			var productsRecommendation = _productDao.GetProductsByIds(numberList)
													.Skip((page - 1) * pageSize)
													.Take(pageSize)
													.ToList();
			totalProductCount = _productDao.GetProductsByIds(numberList).Count();
			totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

			viewModel = new ProductViewModel
			{
				Products = productsRecommendation,
				CurrentPage = page,
				TotalPages = totalPages,
				ProductCategories = productCategories, // Gán danh sách danh mục sản phẩm
				SearchQuery = categoryId.HasValue ? productCategories.FirstOrDefault(c => c.Id == categoryId.Value)?.Name : null
			};

			ViewBag.CategoryId = categoryId;

			return View(viewModel);
		}







		public IActionResult Search(string query, int page = 1)
		{
			const int pageSize = 9;
			int totalProductCount;
			int totalPages;
			ProductViewModel viewModel;

			var productCategories = _productCategoryDao.ListAll().ToList(); // Lấy danh sách danh mục sản phẩm

			if (string.IsNullOrEmpty(query))
			{
				// Nếu không có query, hiển thị tất cả sản phẩm như phương thức Shop
				return RedirectToAction("Shop", new { page });
			}

			var products = _productDao.SearchProductsByName(query)
									  .Skip((page - 1) * pageSize)
									  .Take(pageSize)
									  .ToList();
			totalProductCount = _productDao.SearchProductsByName(query).Count();
			totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

			viewModel = new ProductViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = totalPages,
				ProductCategories = productCategories, // Truyền danh mục sản phẩm vào ViewModel
				SearchQuery = query // Lưu lại query để sử dụng trong View nếu cần
			};

			return View("Shop", viewModel); // Sử dụng view Shop để hiển thị kết quả tìm kiếm
		}


		//public IActionResult Detail(long id)
		//{
		//	var product = _productDao.ViewDetail(id);
		//	ViewBag.Category = _productCategoryDao.ViewDetail(product.CategoryId.Value);
		//	//ViewBag.RelatedProducts = _productDao.ListRelatedProduct(id);
		//	return View(product);
		//}

		public async Task<IActionResult> Detail(long id, int page = 1)
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
			
			if(_favouriteProductDao.CheckFavouriteProduct(_userManager.GetUserId(User), product.Id) == 1)
			{
				ViewBag.Like = "1";
			}
			else
			{
				ViewBag.Like = "0";
			}
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
				UserId = userId,
				Rating = rating,
				ReviewText = reviewText,
				ReviewDate = DateTime.Now
			};

			// Gọi phương thức AddReview để thêm đánh giá vào cơ sở dữ liệu
			_productDao.AddReview(review);

			// Chuyển hướng người dùng trở lại trang chi tiết sản phẩm
			return RedirectToAction("Detail", new { id = id });
		}
		private string RunPythonScript(string scriptName, string inputJson)
		{
            // Đường dẫn tuyệt đối đến script Python
            //Sửa về đúng đường dẫn của máy bản thân
            string scriptPath = @"C:\Users\Admin\Desktop\DOAN_CS\DOAN_COSO\phantichdata_Python\" + scriptName;

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(scriptPath))
			{
				throw new FileNotFoundException($"Script file '{scriptName}' not found in directory.");
			}

			ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";
            //string inputJson1 = JsonConvert.SerializeObject(inputJson);  // yourDataObject là đối tượng chứa dữ liệu bạn muốn gửi

            // Escape chuỗi JSON cho command line
            inputJson = inputJson.Replace("\"", "\\\"");

			// Đặt các đối số cho script Python
			start.Arguments = $"\"{scriptPath}\" \"{inputJson}\"";
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			start.RedirectStandardError = true;

			// Chạy process và đọc kết quả
			using (Process process = Process.Start(start))
			{
				using (StreamReader reader = process.StandardOutput)
				{
					// Đọc kết quả output từ script Python
					string result = reader.ReadToEnd();

					// Đọc và xử lý lỗi nếu có từ script Python
					using (StreamReader errorReader = process.StandardError)
					{
						string errors = errorReader.ReadToEnd();
						if (!string.IsNullOrEmpty(errors))
						{
							throw new Exception("Python script error: " + errors);
						}
					}

					// Trả về kết quả nếu không có lỗi
					return result;
				}
			}
		}

	}
	//public IActionResult AddItem(long productId,int quantity)
	//{
	//	var session = Session[CartSession];
	//}

}
