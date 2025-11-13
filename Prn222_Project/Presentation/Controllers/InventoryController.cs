using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.Interface;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Category;
using Presentation.ViewModel.Data.Inventory;
using Presentation.ViewModel.Data.Product;
using Presentation.ViewModel.Params.Inventory;
using System.Security.Claims;
using System.Text.Json;

namespace Presentation.Controllers {
    [Route("inventory")]
    [Authorize(Roles = "Seller")]
    public class InventoryController : Controller {
        private readonly IInventoryService _invService;
        private readonly ICategoryService _categoryService;

        public InventoryController(IInventoryService invService, ICategoryService categoryService) {
            _invService = invService;
            _categoryService = categoryService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchInventory([FromQuery] SearchParams searchRequest) {
            try {
                var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var categories = await _categoryService.GetAllCategoriesAsync();
                var categoryOptions = categories.Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                categoryOptions.Insert(0, new SelectListItem { Value = "", Text = "All" });

                ViewBag.Categories = categoryOptions;

                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjax) {
                    var inventories = await _invService.SearchInventoryAsync(new SearchRequest {
                        SellerId = sellerId,
                        SearchTerm = searchRequest.SearchTerm,
                        CategoryId = searchRequest.CategoryId,
                        StockStatus = Enum.Parse<StockStatusEnum>(searchRequest.StockStatus.ToString())
                    });

                    string filterHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryFilterPartial", searchRequest, true);

                    string tableHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryTablePartial", inventories.Select(inv => new InventoryViewModel {
                        Id = inv.Id,
                        Product = new ProductViewModel {
                            Id = inv.Product.Id,
                            Title = inv.Product.Title,
                            ImageUrl = inv.Product.ImageUrl,
                            Category = new CategoryViewModel {
                                Id = inv.Product.Category.Id,
                                Name = inv.Product.Category.Name
                            }
                        },
                        Quantity = inv.Quantity,
                        LastUpdated = inv.LastUpdated.HasValue ? inv.LastUpdated.Value : null
                    }).ToList(), true);

                    return Json(new {
                        success = true,
                        html = new {
                            filter = filterHtml,
                            table = tableHtml
                        }
                    });
                }

                return View("Index");

            } catch (Exception ex) {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, new { success = false, message = ex.Message });
                else
                    // (Bạn nên có một trang Error.cshtml đẹp)
                    return StatusCode(500, ex.Message);
            }
        }
    }
}
