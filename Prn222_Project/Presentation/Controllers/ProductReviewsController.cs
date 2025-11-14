using BusinessLogic.DTOs.Request.Pagination;
using BusinessLogic.DTOs.Request.Review;
using BusinessLogic.Interface;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Category;
using Presentation.ViewModel.Data.Inventory;
using Presentation.ViewModel.Data.Pagination;
using Presentation.ViewModel.Data.Product;
using Presentation.ViewModel.Data.ProductReviews;
using Presentation.ViewModel.Params.ProductReviews;
using System.Security.Claims;

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

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductReviews([FromQuery] SearchParams searchRequest) {
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
                    var pageResult = await _reviewService.SearchReviewAsync(new SearchRequest {
                        SellerId = sellerId,
                        SearchTerm = searchRequest.SearchTerm,
                        CategoryId = searchRequest.CategoryId,
                        FromDate = searchRequest.FromDate,
                        ToDate = searchRequest.ToDate,
                        Pagination = new PaginationRequest {
                            PageIndex = searchRequest.Pagination.PageIndex,
                            PageSize = searchRequest.Pagination.PageSize
                        }
                    });

                    string filterHtml = await this.RenderViewAsync("~/Views/ProductReviews/_ProductReviewsFilterPartial", searchRequest, true);

                    string tableHtml = await this.RenderViewAsync("~/Views/ProductReviews/_ProductReviewsTablePartial", pageResult.Items.Select(pr => new ProductReviewsViewModel {
                        Id = pr.Id,
                        StartIndex = (pageResult.PageIndex - 1) * pageResult.PageSize,
                        Product = new ProductViewModel {
                            Id = pr.Product.Id,
                            Title = pr.Product.Title,
                            ImageUrl = pr.Product.ImageUrl,
                            Category = new CategoryViewModel {
                                Id = pr.Product.Category.Id,
                                Name = pr.Product.Category.Name
                            }
                        },
                        Reviewer = pr.Reviewer,
                        Rating = pr.Rating,
                        Comment = pr.Comment,
                        ReviewDate = pr.CreatedAt
                    }).ToList(), true);

                    string paginationHtml = await this.RenderViewAsync(
                 "~/Views/Shared/_PaginationPartial.cshtml", // <-- Dùng Partial CHUNG
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
