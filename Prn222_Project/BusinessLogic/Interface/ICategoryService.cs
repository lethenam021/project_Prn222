using BusinessLogic.DTOs.Response.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface ICategoryService {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    }
}
