using BusinessLogic.Interface;
using DataAccess.IRepo;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class StoreService: IStoreSevice
    {
        private readonly IStoreRepo _repo;

        public StoreService(IStoreRepo repo)
        {
            _repo = repo;
        }

        public async Task<List<Store>> GetAllStoresAsync()
            => await _repo.GetAllStoresAsync();

        public async Task<Store?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task AddAsync(Store store)
            => await _repo.AddAsync(store);

        public async Task UpdateAsync(Store store)
            => await _repo.UpdateAsync(store);

        public async Task DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
