using app.Data;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.IO;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Net.WebSockets;


namespace app.Controllers
{
	
	[Route("api/[controller]")]
	[ApiController]
	public class RecommendationController : Controller
	{
		private readonly ApplicationDbContext _context;
		//private readonly UserManager<IdentityUser> _userManager;

		public RecommendationController(ApplicationDbContext context)
		{
			_context = context;
			//_userManager = userManager;
		}

		[HttpGet]
		public IActionResult GetUserData()
		{


			//var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			//if (string.IsNullOrEmpty(userId))
			//{
			//	return Unauthorized("No UserId found in session");
			//}

			//// Xử lý tiếp theo với `userId`
			//return Ok(new { UserId = userId });

			// Lấy UserId của người dùng đang đăng nhập
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var reviews = _context.ProductReviews
				.Where(r => r.UserId == currentUserId)
				.GroupBy(r => r.ProductId)
				.Select(g => new { ProductId = g.Key, Rating = g.Average(r => r.Rating) })
				.ToList();

			var jsonInput = JsonConvert.SerializeObject(reviews);



			//var data = new[]
			//{
			//	new { ProductId = 4, Rating = 3 },
			//	new { ProductId = 8, Rating = 5 }
			//};

			//string jsonInput = JsonConvert.SerializeObject(data);


			// Gọi tới script Python để tính toán
			var recommendation = RunPythonScript("recommendation_script.py", jsonInput);

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
			var product = _context.Products
                .Where(p => numberList.Contains(p.Id))
                .ToList();
			
            return Ok(product);
		}


		//[HttpPost]

		//public IActionResult GetRecommendations()
		//{
		//	// Lấy UserId của người dùng đang đăng nhập
		//	var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		//	// Truy vấn các đánh giá của người dùng từ cơ sở dữ liệu
		//	var reviews = _context.ProductReviews
		//		.Where(r => r.UserId == currentUserId)
		//		.Select(r => new { r.ProductId, r.Rating })
		//		.ToList();

		//	// Chuyển đổi đánh giá thành định dạng JSON để truyền cho Python
		//	//var jsonInput = JsonConvert.SerializeObject(reviews);

		//	string jsonInput = @"[
		//		{ ""ProductId"": 4, ""Rating"": 3 },
		//		{ ""ProductId"": 8, ""Rating"": 5 }
		//	]";

		//	//string result = RunPythonScript("recommendation_script.py", jsonInput);


		//	// Gọi tới script Python để tính toán
		//	var recommendation = RunPythonScript("recommendation_script.py", jsonInput);

		//	return Ok(new { Recommendation = recommendation });
		//}


		private string RunPythonScript(string scriptName, string inputJson)
		{
            // Đường dẫn tuyệt đối đến script Python
            string scriptPath = @"D:\OneDrive\Hutech\subjectNew\DACS\DOAN_COSO\phantichdata_Python\" + scriptName;

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Script file '{scriptName}' not found in directory.");
            }

            // Thiết lập thông tin cho quá trình chạy script Python
            ProcessStartInfo start = new ProcessStartInfo();
            // Sử dụng Python từ môi trường ảo
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

}
