using app.Areas.Admin.Models;
using app.Common;
using app.Dao;
using app.Models;
using Microsoft.AspNetCore.Mvc;

namespace app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegisterController : Controller
    {

        private readonly UserDao _userDao;
        public RegisterController(UserDao userDao)
        {
            _userDao = userDao;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Action xử lý đăng ký người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại hay chưa
                var existingUser = _userDao.GetByEmail(model.UserName); // Giả sử GetByEmail là phương thức kiểm tra tồn tại của UserDao
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                    return View("Index", model); // Trả về View đăng ký cùng thông báo lỗi
                }

                // Tạo người dùng mới nếu email chưa tồn tại
                var newUser = new User
                {
                    UserName = model.UserName,
                    Password = Encryptor.MD5Hash(model.Password),
                    Status = true
                };

                var result = _userDao.Insert(newUser);
                if (result > 0)
                {
                    return Redirect("~/admin/Login");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng ký thất bại, vui lòng thử lại.");
                }
            }
            return View("Index", model);
        }

    }
}
