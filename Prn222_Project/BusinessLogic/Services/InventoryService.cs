using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.DTOs.Response.Category;
using BusinessLogic.DTOs.Response.Inventory;
using BusinessLogic.DTOs.Response.Product;
using BusinessLogic.Interface;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class InventoryService : IInventoryService {
        private readonly IInventoryRepo _invRepo;

        public InventoryService(IInventoryRepo invRepo) {
            _invRepo = invRepo;
        }

        public async Task<IEnumerable<InventoryResponse>> SearchInventoryAsync(SearchRequest searchRequest) {
            var inventories = await _invRepo.GetInventoriesAsync(searchRequest.SellerId, searchRequest.SearchTerm, searchRequest.CategoryId, searchRequest.StockStatus);

            return inventories.Select(inv => new InventoryResponse {
                Id = inv.Id,
                Product = new ProductResponse {
                    Id = inv.Product!.Id,
                    Title = inv.Product.Title!,
                    Category = new CategoryResponse {
                        Id = inv.Product.Category!.Id,
                        Name = inv.Product.Category.Name!
                    },
                },
                Quantity = inv.Quantity.HasValue ? inv.Quantity.Value : 0,
                LastUpdated = inv.LastUpdated.HasValue ? inv.LastUpdated.Value : null
            });
        }
    }
}
