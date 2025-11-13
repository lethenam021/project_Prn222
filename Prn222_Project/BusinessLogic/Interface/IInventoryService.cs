using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.DTOs.Response.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface IInventoryService {
        Task<IEnumerable<InventoryResponse>> SearchInventoryAsync(SearchRequest searchRequest);

    }
}
