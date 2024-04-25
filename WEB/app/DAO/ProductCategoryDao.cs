using app.Data;
using app.Models;
using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;


namespace Model.Dao
{
    public class ProductCategoryDao
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ProductCategory> ListAll()
        {
            return _context.ProductCategories.Where(x => x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }

        public ProductCategory ViewDetail(long id)
        {
            return _context.ProductCategories.Find(id);
        }

        public long Insert(ProductCategory entity)
        {
            _context.ProductCategories.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public bool Update(ProductCategory entity)
        {
            try
            {
                var category = _context.ProductCategories.Find(entity.Id);
                if (category != null)
                {
                    category.Name = entity.Name;
                    category.MetaTitle = entity.MetaTitle;
                    category.DisplayOrder = entity.DisplayOrder;
                    category.Status = entity.Status;
                    category.ShowOnHome = entity.ShowOnHome;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                return false;
            }
        }

        public IPagedList<ProductCategory> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<ProductCategory> model = _context.ProductCategories;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.MetaTitle.Contains(searchString));
            }

            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }


        public bool Delete(long id)
        {
            try
            {
                var category = _context.ProductCategories.Find(id);
                if (category != null)
                {
                    _context.ProductCategories.Remove(category);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                // Log the exception or handle it as needed
                return false;
            }
        }

    }
}
