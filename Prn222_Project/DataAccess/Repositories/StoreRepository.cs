using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.IRepo;

namespace DataAccess.Repositories
{
    public class StoreRepository : IStoreRepo
    {
        private readonly CloneEbayDbContext _context;
        public StoreRepository(CloneEbayDbContext context) => _context = context;

        // ===== STORE =====
        public async Task<List<Store>> GetAllStoresAsync()
            => await _context.Stores.ToListAsync();

        public async Task<Store?> GetByIdAsync(int id)
            => await _context.Stores.FindAsync(id);

        public async Task AddAsync(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Store store)
        {
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store != null)
            {
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
            }
        }

       
        }
    }