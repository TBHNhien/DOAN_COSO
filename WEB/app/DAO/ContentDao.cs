using app.Data;
using app.Models;
//using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ContentDao
    {
        private readonly ApplicationDbContext _context;

        public ContentDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public Content GetByID(long id)
        {
            return _context.Contents.Find(id);
        }
    }
}
