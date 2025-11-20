using Common.Enums;
using Common.Helpers;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public interface IInventoryRepo {
        Task<PagedResult<Inventory>> GetInventoriesAsync(int sellerId, string? productName, int? categoryId, StockStatusEnum status, int pageIndex, int pageSize);

        Task UpdateQuantityAsync(int productId, int quantity);
    }
}
