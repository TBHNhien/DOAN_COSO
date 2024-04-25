using app.Dao;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Model.Dao;

namespace app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : BaseController
    {

            private readonly ProductCategoryDao _productCategoryDao;

            public ProductCategoryController(ProductCategoryDao  productCategoryDao)
            {
                _productCategoryDao = productCategoryDao;
            }




        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {


            var model = _productCategoryDao.ListAllPaging(searchString, page, pageSize);

            ViewBag.SearchString = searchString;
            return View(model);
        }

        public ActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(ProductCategory category)
            {
                if (ModelState.IsValid)
                {
                    long id = _productCategoryDao.Insert(category);
                    if (id > 0)
                    {
                    TempData["SuccessMessage"] = "Thêm loại sản phẩm thành công";
                    return Redirect("~/admin/ProductCategory");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cannot insert product category");
                    }
                }
                return View(category);
            }

            public ActionResult Edit(long id)
            {
                var category = _productCategoryDao.ViewDetail(id);
                return View(category);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(ProductCategory category)
            {
                if (ModelState.IsValid)
                {
                    var result = _productCategoryDao.Update(category);
                    if (result)
                    {
                    TempData["SuccessMessage"] = "Sửa loại sản phẩm thành công";
                    return Redirect("~/admin/ProductCategory");
                }
                    else
                    {
                        ModelState.AddModelError("", "Cannot update product category");
                    }
                }
                return View(category);
            }

            public ActionResult Delete(long id)
            {
                var result = _productCategoryDao.Delete(id);
                if (result)
                {
                    return Redirect("~/admin/ProductCategory");
                }
                else
                {
                    ModelState.AddModelError("", "Cannot delete product category");
                // Return to a view that displays the error
                return Redirect("~/admin/ProductCategory");
                }
            }
        }
}
