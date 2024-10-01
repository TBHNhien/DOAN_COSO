using Model.Dao;
using Microsoft.AspNetCore.Mvc;
using app.Models; // Sửa lại tên namespace cho phù hợp với Product model của bạn

namespace app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : BaseController
    {
        private readonly ProductDao _productDao;

        public ProductController(ProductDao productDao)
        {
            _productDao = productDao;
        }

        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var model = _productDao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                long id = _productDao.Insert(product);
                if (id > 0)
                {
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công";
                    return Redirect("~/admin/Product");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể thêm sản phẩm");
                }
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult Edit(long id)
        {
            var product = _productDao.ViewDetail(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                bool result = _productDao.Update(product);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công";
                    return Redirect("~/admin/Product");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật sản phẩm");
                }
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            bool result = _productDao.Delete(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm";
            }
            return Redirect("~/admin/Product");
        }
    }
}
