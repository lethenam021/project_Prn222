using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.DTOs.Response.Inventory;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface IInventoryService {
        Task<PagedResult<InventoryResponse>> SearchInventoryAsync(SearchRequest searchRequest);
        Task UpdateInventoryQuantityAsync(UpdateQuantityRequest updateInventoryRequest);
    }
}
