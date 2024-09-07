using Microsoft.AspNetCore.Mvc;

namespace app.Areas.Admin.Controllers
{
	public class CopilotController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

	}
}
