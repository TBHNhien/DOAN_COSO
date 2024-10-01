using app.BLL;
using app.Data;
using app.Helpers;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using app.Services;
using app.Models.Order_MoMo;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;


namespace app.Controllers
{
	
	public class BillsController : Controller
	{

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private IMomoService _momoService;
		//private readonly ShoppingCart _shoppingCart;

		public BillsController(ApplicationDbContext context,UserManager<IdentityUser> userManager, IMomoService momoService)
        {
            _context = context;
            _userManager = userManager;
            _momoService = momoService;

		}



		[HttpPost]
		public IActionResult UpdateCartQuantity(int productId, int quantity)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart != null && quantity > 0)
			{
				cart.UpdateItemQuantity(productId, quantity);
				HttpContext.Session.SetObjectAsJson("Cart", cart);
			}
			return Json(new { status = "success", message = "Quantity updated." });
		}

		[HttpGet]
		public IActionResult GetTotalPrice()
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart != null)
			{
				decimal totalPrice = cart.ComputeTotalPrice(); // Hàm này bạn cần phải triển khai trong class ShoppingCart
				return Json(totalPrice);
			}
			return Json(0); // Trả về 0 nếu không có giỏ hàng hoặc giỏ hàng trống
		}




		public Product GetProductFromDatabase(int productId)
		{
			var product = _context.Products.SingleOrDefault(p => p.Id == productId);
			return product;
		}

		public IActionResult AddToCart(int productId, int quantity)
		{
			// Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
			var product = GetProductFromDatabase(productId);

			var cartItem = new CartItem
			{
				ProductId = productId,
				Name = product.Name,
				Price = (decimal)product.Price,
				Quantity = quantity,
                Image = product.Image
            };
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			cart.AddItem(cartItem);
			HttpContext.Session.SetObjectAsJson("Cart", cart);
			return RedirectToAction("Index");
		}

		[Authorize]
		public IActionResult Index()
		{

			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			return View(cart);
		}


		//[HttpPost]
		//public async Task<IActionResult> Index(Order order)
		//{
		//	return View();
		//}

		public IActionResult Checkout()
		{
			return View(new Order());
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Checkout(Order order)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart == null || !cart.Items.Any())
			{
				// Xử lý giỏ hàng trống...
				return RedirectToAction("Index");
			}
			var user = await _userManager.GetUserAsync(User);
			order.UserId = user.Id;
			order.CreatedDate = DateTime.UtcNow;
			order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
			order.OrderDetails = cart.Items.Select(i => new OrderDetail
			{
				ProductID = i.ProductId,
				Quantity = i.Quantity,
				Price = i.Price
			}).ToList();
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();
			HttpContext.Session.Remove("Cart");
            //return View("OrderCompleted", order.ID); // Trang xác nhận hoàn thành đơn hàng

            ViewBag.OrderID = order.ID;
            ViewBag.TotalPrice = order.TotalPrice;
            return View("MoMoIndex");

		}
		public IActionResult Cart()
		{
			return View();
		}
		public IActionResult Payment()
		{
			return View();
		}

        public IActionResult RemoveFromCart(int productId)
        {
            var cart =HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);

            // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
			HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }



        //public IActionResult Checkout(int productId, int quantity)
        //{
        //	// Xử lý số lượng hàng ở đây

        //	return RedirectToAction("Payment"); // Chuyển hướng đến trang thanh toán
        //}


        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderMoMoInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }

    }
}
