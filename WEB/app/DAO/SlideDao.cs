using app.Data;
using app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class SlideDao
    {
        private readonly ApplicationDbContext _context;

        public SlideDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Slide> ListAll() 
        {
            return _context.Slides.Where(x => x.Status == true).OrderBy(y => y.DisplayOrder).ToList();
        }


    }
}
