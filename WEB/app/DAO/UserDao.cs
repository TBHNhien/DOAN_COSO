using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.Data;
using app.Models;

using X.PagedList;

namespace app.Dao
{
    public class UserDao
    {
        private readonly ApplicationDbContext _context;

        public UserDao(ApplicationDbContext context)
        {
            _context = context;
        }


        public long Insert(User entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public bool Update(User entity, string oldPassword)
        {
            try
            {
                var user = _context.Users.Find(entity.Id);
                user.Name = entity.Name;
                if (!string.IsNullOrEmpty(entity.Password) && entity.Password != oldPassword)
                {
                    user.Password = entity.Password;
                }
                user.Address = entity.Address;
                user.Email = entity.Email;
                user.ModifiedBy = entity.ModifiedBy;
                user.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<User> ListAllPaging(string searchString,int page , int pageSize)
        {
            IQueryable<User> model = _context.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.UserName.Contains(searchString) || x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page,pageSize);

        }

        public User GetById (string userName)
        {
            return _context.Users.SingleOrDefault(x => x.UserName == userName);
        }

        public User ViewDetail(long id)
        {
            return _context.Users.Find(id);
        }

        public int Login (string userName , string passWord)
        {
            var result = _context.Users.SingleOrDefault(x => x.UserName == userName );
            if(result == null)
            {
                return 0;
            }
            else
            {
                if(result.Status == false)
                {
                    return -1;
                }
                else
                {
                    if (result.Password == passWord)
                        return 1;
                    else
                        return -2;
                }
            }
        }
        public bool ChangeStatus(long id)
        {
            var user = _context.Users.Find(id);
            user.Status = !user.Status;

            _context.SaveChanges();
            return user.Status;
        }

        public bool Delete(long id)
        {
            try
            {
                var user = _context.Users.Find(id);
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }

        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == email); // Kiểm tra email đã tồn tại
        }

    }
}
