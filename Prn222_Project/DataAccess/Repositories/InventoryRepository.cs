using Common.Enums;
using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class InventoryRepository : IInventoryRepo {
        private readonly CloneEbayDbContext _context;

        public InventoryRepository(CloneEbayDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesAsync(int sellerId, string? productName, int? categoryId, StockStatusEnum status) {
            var query = _context.Inventories.Include(i => i.Product).AsQueryable();

            if (sellerId > 0) {
                query = query.Where(i => i.Product != null && i.Product.SellerId == sellerId);
            }

            if (!string.IsNullOrEmpty(productName)) {
                query = query.Where(i => i.Product != null && i.Product.Title!.Contains(productName));
            }

            if (categoryId.HasValue && categoryId.Value > 0) {
                query = query.Where(i => i.Product != null && i.Product.CategoryId == categoryId);
            }

            switch (status) {
                case StockStatusEnum.InStock:
                query = query.Where(i => i.Quantity > 0);
                break;
                case StockStatusEnum.OutOfStock:
                query = query.Where(i => i.Quantity == 0);
                break;
                case StockStatusEnum.LowStock:
                query = query.Where(i => i.Quantity > 0 && i.Quantity < 5);
                break;
                case StockStatusEnum.All:
                default:
                break;
            }

            return await query.OrderBy(i => i.Id).ToListAsync();
        }
    }
}
