using app.Data;
using app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Model.Dao
{
    public class ProductDao
    {
        private readonly ApplicationDbContext _context;

        public ProductDao(ApplicationDbContext context)
        {
            _context = context;
        }

		public ProductDao()
		{
		}

		/// <summary>
		/// Get list product by category
		/// </summary>
		/// <param name="categoryID"></param>
		/// <returns></returns>
		public List<Product> ListByCategoryId(long categoryID ,  ref int totalRecord , int pageIndex=1 , int pageSize=2)
        {
            totalRecord = _context.Products.Where(x => x.CategoryId == categoryID).Count(); //lấy tổng các bản ghi
            var model = _context.Products.Where(x => x.CategoryId == categoryID).OrderByDescending(x=>x.CreatedDate).Skip((pageIndex - 1)* pageSize).Take(pageSize).ToList();

            return model;
        }

		/// <summary>
		/// List new product
		/// </summary>
		/// <param name="top"></param>
		/// <returns></returns>

		public List<Product> ListProduct()
		{
			return _context.Products.ToList();
		}

        public List<Product> ListAll()
        {
            return _context.Products.OrderBy(x => x.Name).ToList();
        }

        public List<Product> ListNewProduct(int top)
        {
            return _context.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<Product> ListFeatureProduct(int top)
        {
            return _context.Products.Where(x=>x.TopHot!=null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<Product> ListRelatedProduct(long productId)
        {
            var product = _context.Products.Find(productId);
            return _context.Products.Where(x => x.Id != productId && x.CategoryId == product.CategoryId ).ToList();
        }

        public Product ViewDetail(long id)
        {
            return _context.Products.Find(id);
        }

		// Phương thức mới để lấy đánh giá của sản phẩm
		public IEnumerable<ProductReview> ListReviewsByProductId(long productId)
		{
			// Sử dụng LINQ để truy vấn cơ sở dữ liệu
			var reviews = _context.ProductReviews
								  .Where(r => r.ProductId == productId)
								  .OrderByDescending(r => r.ReviewDate) // Sắp xếp theo ngày đánh giá gần nhất
								  .ToList(); // Chuyển kết quả thành danh sách

			return reviews;
		}

		// Phương thức thêm đánh giá mới
		public void AddReview(ProductReview review)
		{
			if (review == null)
			{
				throw new ArgumentNullException(nameof(review));
			}

			// Thêm đánh giá vào DbSet<ProductReview>
			_context.ProductReviews.Add(review);

			// Lưu các thay đổi vào cơ sở dữ liệu
			_context.SaveChanges();
		}


		public long Insert(Product entity)
        {
            _context.Products.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public bool Update(Product entity)
        {
            try
            {
                var product = _context.Products.Find(entity.Id);
                if (product != null)
                {
                    product.Name = entity.Name;
                    product.Code = entity.Code;
                    product.MetaTitle = entity.MetaTitle;
                    product.Description = entity.Description;
                    product.Image = entity.Image;
                    product.Price = entity.Price;
                    product.Quantity = entity.Quantity;
                    product.CategoryId = entity.CategoryId; // Ensure you have the correct foreign key property
                    product.Warranty = entity.Warranty;


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

        public IPagedList<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = _context.Products;

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
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
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
