using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectPRN222.Services
{
    public class StoreService
    {
        private readonly CloneEbayContext _db;

        // state trong bộ nhớ
        private readonly List<Store> _stores = new();

        // expose read-only
        public IReadOnlyList<Store> Stores => _stores;

        public event Action? OnChange;

        public StoreService(CloneEbayContext db) => _db = db;

        // Khởi tạo/refresh state
        public async Task InitializeAsync(CancellationToken ct = default)
        {
            if (_stores.Count == 0)
            {
                var rows = await _db.Stores.AsNoTracking().OrderBy(s => s.storeName).ToListAsync(ct);
                _stores.Clear();
                _stores.AddRange(rows);
                Notify();
            }
        }

        public async Task RefreshAsync(CancellationToken ct = default)
        {
            var rows = await _db.Stores.AsNoTracking().OrderBy(s => s.storeName).ToListAsync(ct);
            _stores.Clear();
            _stores.AddRange(rows);
            Notify();
        }

        public Store? Find(int id) => _stores.FirstOrDefault(s => s.id == id);

        public async Task<Store?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Stores.FirstOrDefaultAsync(s => s.id == id, ct);

        public async Task<User?> GetUserByStoreIdAsync(int storeId, CancellationToken ct = default)
        {
            return await _db.Stores
                .Where(s => s.id == storeId)
                .Select(s => s.seller)
                .FirstOrDefaultAsync(ct);
        }
        public async Task<List<Product>?> GetProductAsync(int sellerId, CancellationToken ct = default)
        {
            return await _db.Set<Product>()
       .Where(su => su.sellerId == sellerId)
       .AsNoTracking()
       .ToListAsync(ct);
        }

        public async Task<Store> AddAsync(Store input, CancellationToken ct = default)
        {
            // server-side validations nhẹ (ví dụ tên không rỗng)
            if (string.IsNullOrWhiteSpace(input.storeName))
                throw new InvalidOperationException("storeName is required");

            _db.Stores.Add(input);
            await _db.SaveChangesAsync(ct);

            // update state
            _stores.Add(input);
            Notify();
            return input;
        }

        public async Task<Store> UpdateAsync(Store input, CancellationToken ct = default)
        {
            var entity = await _db.Stores.FirstOrDefaultAsync(s => s.id == input.id, ct)
                ?? throw new KeyNotFoundException("Store not found");

            entity.sellerId = input.sellerId;
            entity.storeName = input.storeName;
            entity.description = input.description;
            entity.bannerImageURL = input.bannerImageURL;

            await _db.SaveChangesAsync(ct);

            // update state in-memory
            var idx = _stores.FindIndex(s => s.id == entity.id);
            if (idx >= 0) _stores[idx] = entity;
            else _stores.Add(entity);

            Notify();
            return entity;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Stores.FirstOrDefaultAsync(s => s.id == id, ct)
                ?? throw new KeyNotFoundException("Store not found");

            _db.Stores.Remove(entity);
            await _db.SaveChangesAsync(ct);

            _stores.RemoveAll(s => s.id == id);
            Notify();
        }

        private void Notify() => OnChange?.Invoke();
    }
}
