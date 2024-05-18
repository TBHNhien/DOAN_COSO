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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string reviews = _productDao.GetReviews(currentUserId);
            const int pageSize = 9;
			int totalProductCount;
			int totalPages;
			ProductViewModel viewModel;
			
            if (!User.Identity.IsAuthenticated || reviews == "[]")
            {

                
                var products = _productDao.ListProduct()
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList();
                totalProductCount = _productDao.ListProduct().Count();
                totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

                viewModel = new ProductViewModel
                {
                    Products = products,
                    CurrentPage = page,
                    TotalPages = totalPages
                };

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
            /*var product = _context.Products
                .Where(p => numberList.Contains(p.Id))
                .ToList();*/
			var productsRecommendation = _productDao.GetProductsByIds(numberList)
													.Skip((page - 1) * pageSize)
													.Take(pageSize)
													.ToList(); ;
            totalProductCount = _productDao.GetProductsByIds(numberList).Count();
            totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

            viewModel = new ProductViewModel
            {
                Products = productsRecommendation,
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
            string scriptPath = @"D:\OneDrive\Hutech\subjectNew\DACS\DOAN_COSO\phantichdata_Python\" + scriptName;

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Script file '{scriptName}' not found in directory.");
            }

            // Thiết lập thông tin cho quá trình chạy script Python
            ProcessStartInfo start = new ProcessStartInfo();
            // Sử dụng Python từ môi trường ảo
			//Ở đây thay bằng "python" để chạy nhớ bỏ dấu @ đi
            start.FileName = @"D:\OneDrive\Hutech\subjectNew\DACS\DOAN_COSO\phantichdata_Python\myenv\Scripts\python.exe";

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
