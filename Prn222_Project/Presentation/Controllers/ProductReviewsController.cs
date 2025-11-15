using BusinessLogic.DTOs.Request.Pagination;
using BusinessLogic.DTOs.Request.Review;
using BusinessLogic.Interface;
using Common.Helpers; // (Cần cho PagedResult)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Category; // (Cần cho CategoryViewModel)
using Presentation.ViewModel.Data.Pagination;
using Presentation.ViewModel.Data.Product;
using Presentation.ViewModel.Data.ProductReviews;
using Presentation.ViewModel.Params.ProductReviews; // (Namespace params của UI)
using System; // (Cần cho Exception)
using System.Linq; // (Cần cho .Select)
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks; // (Cần cho async)

namespace Presentation.Controllers {
    [Route("product-reviews")]
    [Authorize(Roles = "Seller")]
    public class ProductReviewsController : Controller {
        private readonly IReviewService _reviewService;
        private readonly ICategoryService _categoryService;

        public ProductReviewsController(IReviewService reviewService, ICategoryService categoryService) {
            _reviewService = reviewService;
            _categoryService = categoryService;
        }

        // --- ACTION 1: Tải trang Index (chỉ tải Categories) ---
        [HttpGet("")] // (Trang chính)
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

        // --- ACTION 2 (AJAX CẤP 1 - SUMMARY) ---
        [HttpGet("search-summaries")]
        public async Task<IActionResult> SearchSummaries([FromQuery] SearchSummaryParams searchParams) {
            try {
                var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var request = new SearchSummaryRequest {
                    SellerId = sellerId,
                    SearchTerm = searchParams.SearchTerm,
                    CategoryId = searchParams.CategoryId,
                    Pagination = new PaginationRequest {
                        PageIndex = searchParams.Pagination.PageIndex,
                        PageSize = searchParams.Pagination.PageSize
                    }
                };

                // 2. Gọi Service Cấp 1
                var pageResult = await _reviewService.SearchProductSummariesAsync(request);

                // 3. Map Business DTO (ProductReviewSummaryResponse) sang UI VM (ProductReviewSummaryViewModel)
                var viewModels = pageResult.Items.Select((pr, index) => new ProductReviewSummaryViewModel {
                    StartIndex = (pageResult.PageIndex - 1) * pageResult.PageSize,
                    Product = new ProductViewModel {
                        Id = pr.Product.Id,
                        Title = pr.Product.Title,
                        ImageUrl = pr.Product.ImageUrl
                    },
                    AverageRating = pr.AverageRating,
                    TotalReviews = pr.TotalReviews
                }).ToList();

                // 4. Render Partial Views
                string tableHtml = await this.RenderViewAsync(
                    "~/Views/ProductReviews/_ProductReviewSummaryTablePartial",
                    viewModels,
                    true);

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
                        summaryTable = tableHtml,
                        pagination = paginationHtml
                    }
                });
            } catch (Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // --- ACTION 3 (AJAX CẤP 2 - DETAIL) ---
        [HttpGet("get-details")]
        public async Task<IActionResult> GetProductReviewDetails([FromQuery] SearchDetailParams searchParams) {
            try {
                var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var request = new SearchDetailRequest {
                    SellerId = sellerId,
                    ProductId = searchParams.ProductId,
                    FromDate = searchParams.FromDate,
                    ToDate = searchParams.ToDate,
                    Pagination = new PaginationRequest {
                        PageIndex = searchParams.Pagination.PageIndex,
                        PageSize = searchParams.Pagination.PageSize
                    }
                };

                // 2. Gọi Service Cấp 2
                var pageResult = await _reviewService.GetProductDetailsAsync(request);

                // 3. Map Business DTO (ProductReviewDetailResponse) sang UI VM (ProductReviewDetailViewModel)
                var viewModels = pageResult.Items.Select((pr, index) => new ProductReviewDetailViewModel {
                    Id = pr.Id, // (Cần thêm Id vào ViewModel để "Send Feedback")
                    StartIndex = (pageResult.PageIndex - 1) * pageResult.PageSize,
                    Reviewer = pr.Reviewer,
                    Rating = pr.Rating,
                    Comment = pr.Comment,
                    ReviewDate = pr.ReviewDate
                }).ToList();

                // 4. Tạo PagedResult cho UI (bao gồm AdditionalData từ Service)
                var uiPageResult = new PagedResult<ProductReviewDetailViewModel> {
                    Items = viewModels,
                    PageIndex = pageResult.PageIndex,
                    PageSize = pageResult.PageSize,
                    TotalRecord = pageResult.TotalRecord,
                    AdditionalData = pageResult.AdditionalData // (Pass-through)
                };

                // 5. Render *toàn bộ* Partial Cấp 2 (vỏ bọc)
                string detailHtml = await this.RenderViewAsync(
                    "~/Views/ProductReviews/Detail/_ProductReviewDetailPartial",
                    uiPageResult,
                    true);


                return Json(new {
                    success = true,
                    html = detailHtml
                });
            } catch (Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}