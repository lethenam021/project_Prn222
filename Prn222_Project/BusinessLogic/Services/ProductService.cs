using BusinessLogic.Interface;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ProductService :IProductService
    {
        private readonly IProductRepo _repo;
        public ProductService(IProductRepo repo)
        {
            _repo = repo;
        }
        public async Task<List<DataAccess.Models.Product>> GetAllProductsAsync()
            => await _repo.GetAllProductsAsync();
        public async Task<DataAccess.Models.Product?> GetByIdAsync(int id)
            => await  _repo.GetByIdAsync(id);
        public async Task AddAsync(DataAccess.Models.Product product)
            => await _repo.AddAsync(product);
        public async Task UpdateAsync(DataAccess.Models.Product product)
            => await _repo.UpdateAsync(product);
        public async Task DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
