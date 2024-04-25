using app.Data;
using app.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    
    public class MenuDao
    {
        private readonly ApplicationDbContext _context;

        public MenuDao(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Menu> ListByGroupId(int groupId)
        {

            return _context.Menus.Where(x => x.TypeId == groupId && x.Status == true).OrderBy(x=>x.DisplayOrder).ToList();
        }

    }
}
