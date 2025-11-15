using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class CategoryRepository : ICategoryRepo {
        private readonly CloneEbayDbContext _context;

        public CategoryRepository(CloneEbayDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() {
            return await _context.Categories.ToListAsync();
        }
    }
}
