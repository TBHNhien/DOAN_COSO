//using Model.EF;
using app.Data;
using app.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class CategoryDao
    {
        private readonly ApplicationDbContext _context;

        public CategoryDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Category> ListAll()
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["OnlineShopDbContext"].ToString();

            return _context.Categories.Where(x => x.Status == true).ToList();
        }

        public ProductCategory ViewDetail(long id)
        {
            return _context.ProductCategories.Find(id);
        }
    }
}
