using Model.Dao;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using Microsoft.IdentityModel.Tokens; // Sửa lại tên namespace cho phù hợp với Product model của bạn
using X.PagedList;
using X.PagedList.Mvc.Core;


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
        public IActionResult Create(Product product, IFormFile ImageUrl, List<IFormFile> OtherImageUrls)
        {
            if (ModelState.IsValid)
            {
                
                product.MetaTitle = product.Name.Replace(" ", "-").ToLower();
                product.Description = "Sản phẩm đẹp";
                product.CategoryId = 1;

                if (ImageUrl != null)
                    product.Image = SaveImage(ImageUrl);
                if (!OtherImageUrls.IsNullOrEmpty())
                {
                    string temp = "<images>";
                    foreach(var otherImageUrl in OtherImageUrls)
                    {
                        temp = temp + ("<image>/images/" + otherImageUrl.FileName + "</image>");
                        SaveImage(otherImageUrl);
                    }
                    temp += "</images>";
                    product.MoreImages = temp;
                }
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
        //Hàm saveImage
        private string SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối 
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
        public IActionResult Edit(Product product, IFormFile ImageUrl, List<IFormFile> OtherImageUrls)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("OtherImageUrls");
            if (ModelState.IsValid)
            {
                var existingProduct = _productDao.ViewDetail(product.Id);
                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (ImageUrl == null)
                {
                    product.Image = existingProduct.Image;
                }
                else
                {
                    // Lưu hình ảnh mới 
                    product.Image = SaveImage(ImageUrl);
                }
                //Lưu các ảnh mới
                if (!OtherImageUrls.IsNullOrEmpty())
                {
                    
                    string temp = "<images>";
                    foreach (var otherImageUrl in OtherImageUrls)
                    {
                        temp = temp + ("<image>/images/" + otherImageUrl.FileName + "</image>");
                        SaveImage(otherImageUrl);
                    }
                    temp += "</images>";
                    product.MoreImages = temp;
                }
                else
                {
                    // Giữ nguyên các thông tin ảnh nếu không có các ảnh mới được tải lên
                    
                    product.MoreImages = existingProduct.MoreImages;
                }
                product.MetaTitle = existingProduct.MetaTitle;
                product.CategoryId = existingProduct.CategoryId;
                product.Description = "Giày được thiết kế dáng buộc dây năng động,mặt giày vải dệt dày dặn ,viền ép nhiệt phong cách hiện đại,màu sắc khỏe khoắn. Đặc biệt sản phẩm sử dụng chất liệu cao cấp có độ bền tối ưu giúp bạn thoải mái trong mọi hoàn cảnh. Giày thoáng khí cả mặt trong lẫn mặt ngoài khiến người mang luôn cảm thấy dễ chịu dù hoạt động trong thời gian dài.";
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
