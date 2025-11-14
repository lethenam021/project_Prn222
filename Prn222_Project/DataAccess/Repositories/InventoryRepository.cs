using Common.Enums;
using Common.Helpers;
using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class InventoryRepository : IInventoryRepo {
        private readonly CloneEbayDbContext _context;

        public InventoryRepository(CloneEbayDbContext context) {
            _context = context;
        }


        public async Task<PagedResult<Inventory>> GetInventoriesAsync(int sellerId, string? productName, int? categoryId, StockStatusEnum status, int pageIndex, int pageSize) {
            var query = _context.Inventories
               .Include(i => i.Product)                // BẮT BUỘC
                   .ThenInclude(p => p.Category)   // <-- SỬA LỖI (Thêm dòng này)
               .AsNoTracking()
               .AsQueryable(); // (AsQueryable() không cần thiết sau AsNoTracking())


            // 2. LỌC THEO SELLER
            // (Thêm kiểm tra 'Product != null' để phòng trường hợp
            //  dữ liệu inventory bị lỗi (không có product liên kết))
            query = query.Where(i => i.Product != null && i.Product.SellerId == sellerId);

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

            var totalRecord = await query.CountAsync();

            var items = await query.OrderBy(i => i.Id)
                                   .Skip((pageIndex - 1) * pageSize) // Bỏ qua trang trước
                                   .Take(pageSize) // Lấy trang hiện tại
                                   .ToListAsync();
            return new PagedResult<Inventory> {
                Items = items,
                TotalRecord = totalRecord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task UpdateQuantityAsync(int productId, int quantity) {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory != null) {
                inventory.Quantity = quantity;
                inventory.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
