using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.DTOs.Response.Category;
using BusinessLogic.DTOs.Response.Inventory;
using BusinessLogic.DTOs.Response.Product;
using BusinessLogic.Interface;
using Common.Helpers;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class InventoryService : IInventoryService {
        private readonly IInventoryRepo _invRepo;

        public InventoryService(IInventoryRepo invRepo) {
            _invRepo = invRepo;
        }

        public async Task<PagedResult<InventoryResponse>> SearchInventoryAsync(SearchRequest searchRequest) {
            var pagedInventories = await _invRepo.GetInventoriesAsync(
                searchRequest.SellerId,
                searchRequest.SearchTerm,
                searchRequest.CategoryId,
                searchRequest.StockStatus,
                searchRequest.Pagination.PageIndex, // <-- Mới
                searchRequest.Pagination.PageSize   // <-- Mới
            );
            Console.WriteLine(JsonSerializer.Serialize(searchRequest));
            // 2. Map Items
            var responseItems = pagedInventories.Items.Select(inv => new InventoryResponse {
                Id = inv.Id,
                Product = new ProductResponse {
                    Id = inv.Product!.Id,
                    Title = inv.Product.Title!,
                    Category = new CategoryResponse {
                        Id = inv.Product.Category!.Id,
                        Name = inv.Product.Category.Name!
                    },
                    ImageUrl = inv.Product.Images ?? null!
                },
                Quantity = inv.Quantity.HasValue ? inv.Quantity.Value : 0,
                LastUpdated = inv.LastUpdated.HasValue ? inv.LastUpdated.Value : null
            }).ToList();

            Console.WriteLine(JsonSerializer.Serialize(responseItems));
            // 3. Trả về PagedResult DTO
            return new PagedResult<InventoryResponse> {
                Items = responseItems,
                TotalRecord = pagedInventories.TotalRecord,
                PageIndex = pagedInventories.PageIndex,
                PageSize = pagedInventories.PageSize
            };
        }

        public async Task UpdateInventoryQuantityAsync(UpdateQuantityRequest updateQuantityRequest) {
            await _invRepo.UpdateQuantityAsync(updateQuantityRequest.ProductId, updateQuantityRequest.Quantity);
        }
    }
}
