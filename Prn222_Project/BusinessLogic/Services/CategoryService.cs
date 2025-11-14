using BusinessLogic.DTOs.Response.Category;
using BusinessLogic.Interface;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class CategoryService : ICategoryService {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryService(ICategoryRepo categoryRepo) {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync() {
            var categories = await _categoryRepo.GetAllCategoriesAsync();

            return categories.Select(cat => new CategoryResponse {
                Id = cat.Id,
                Name = cat.Name!
            });
        }
    }
}
