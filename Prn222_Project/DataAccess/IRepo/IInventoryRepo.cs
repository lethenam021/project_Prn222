using Common.Enums;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public interface IInventoryRepo {
        Task<IEnumerable<Inventory>> GetInventoriesAsync(int sellerId, string? productName, int? categoryId, StockStatusEnum status);
    }
}
