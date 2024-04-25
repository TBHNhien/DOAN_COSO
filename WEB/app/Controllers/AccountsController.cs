using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
	public class AccountsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}

        public IActionResult Signup()
        {
            return View();
        }
    }
}
