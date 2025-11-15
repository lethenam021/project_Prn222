using BusinessLogic.DTOs.Request.Inventory;
using BusinessLogic.DTOs.Request.Pagination;
using BusinessLogic.Interface;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Category;
using Presentation.ViewModel.Data.Inventory;
using Presentation.ViewModel.Data.Pagination;
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

        [HttpGet("")]
        public async Task<IActionResult> Index() {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryOptions = categories.Select(c => new SelectListItem {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            categoryOptions.Insert(0, new SelectListItem { Value = "", Text = "All" });

            ViewBag.Categories = categoryOptions;
            return View("Index");
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

                // (SỬA) 3. Nếu LÀ full page load (F5), trả về View chính
                if (!isAjax) {
                    // Trả về View("Index").
                    // Javascript (inventory.js) trong Index.cshtml
                    // sẽ tự động trigger một AJAX request mới
                    // để tải đúng dữ liệu (sử dụng các param trên URL).
                    return View("Index");
                }

                // 1. (SỬA) Map Paging vào Service Request
                var serviceRequest = new SearchRequest {
                    SellerId = sellerId,
                    SearchTerm = searchRequest.SearchTerm,
                    CategoryId = searchRequest.CategoryId,
                    StockStatus = Enum.Parse<StockStatusEnum>(searchRequest.StockStatus.ToString()),
                    Pagination = new PaginationRequest {
                        PageIndex = searchRequest.Pagination.PageIndex,
                        PageSize = searchRequest.Pagination.PageSize
                    }
                };

                // 2. (SỬA) Nhận PagedResult
                var pageResult = await _invService.SearchInventoryAsync(serviceRequest);

                // 3. (SỬA) Map PagedResult.Items sang ViewModel
                var viewModels = pageResult.Items.Select((inv, index) => new InventoryViewModel {
                    // (THÊM MỚI) Thêm StartIndex để đếm số thứ tự
                    StartIndex = (pageResult.PageIndex - 1) * pageResult.PageSize,
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
                }).ToList();

                // 4. (SỬA) Render 3 partials
                string filterHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryFilterPartial.cshtml", searchRequest, true);
                string tableHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryTablePartial.cshtml", viewModels, true);

                // (THÊM MỚI) Render Paging
                string paginationHtml = await this.RenderViewAsync(
                    "~/Views/Shared/_PaginationPartial",
                    new PaginationViewModel {
                        PageIndex = pageResult.PageIndex,
                        PageSize = pageResult.PageSize,
                        TotalRecords = pageResult.TotalRecord
                    },
                    true);

                return Json(new {
                    success = true,
                    html = new {
                        filter = filterHtml,
                        table = tableHtml,
                        pagination = paginationHtml // (THÊM MỚI)
                    }
                });

            } catch (Exception ex) {
                // (Giữ nguyên)
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, new { success = false, message = ex.Message });
                else
                    return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("update-quantity")]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity, [FromQuery] SearchParams searchParams) {
            try {
                var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                await _invService.UpdateInventoryQuantityAsync(new UpdateQuantityRequest {
                    ProductId = productId,
                    Quantity = quantity
                });

                var serviceRequest = new SearchRequest {
                    SellerId = sellerId,
                    SearchTerm = searchParams.SearchTerm,
                    CategoryId = searchParams.CategoryId,
                    StockStatus = Enum.Parse<StockStatusEnum>(searchParams.StockStatus.ToString()),
                    Pagination = new PaginationRequest {
                        PageIndex = searchParams.Pagination.PageIndex,
                        PageSize = searchParams.Pagination.PageSize
                    }
                };

                var pageResult = await _invService.SearchInventoryAsync(serviceRequest);

                // (SỬA) Map PagedResult.Items
                var viewModels = pageResult.Items.Select((inv, index) => new InventoryViewModel {
                    StartIndex = (pageResult.PageIndex - 1) * pageResult.PageSize,
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
                }).ToList();

                // (SỬA) Render cả 3 partials
                // (Cần tải lại filter để giữ state, mặc dù không thay đổi)
                string filterHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryFilterPartial.cshtml", searchParams, true);
                string tableHtml = await this.RenderViewAsync("~/Views/Inventory/_InventoryTablePartial.cshtml", viewModels, true);
                string paginationHtml = await this.RenderViewAsync(
                    "~/Views/Shared/_PaginationPartial.cshtml",
                    new PaginationViewModel {
                        PageIndex = pageResult.PageIndex,
                        PageSize = pageResult.PageSize,
                        TotalRecords = pageResult.TotalRecord
                    },
                    true);

                return Json(new {
                    success = true,
                    html = new {
                        filter = filterHtml,
                        table = tableHtml,
                        pagination = paginationHtml
                    }
                });
            } catch (Exception ex) {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
