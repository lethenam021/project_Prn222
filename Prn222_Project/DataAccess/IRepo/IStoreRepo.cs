using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IStoreRepo
    {
        Task<List<Store>> GetAllStoresAsync();
        Task<Store?> GetByIdAsync(int id);
        Task AddAsync(Store store);
        Task UpdateAsync(Store store);
        Task DeleteAsync(int id);
    }
}
