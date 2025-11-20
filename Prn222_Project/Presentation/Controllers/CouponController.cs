using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BusinessLogic.Interface;
using DataAccess.Models;
using Presentation.ViewModel;

namespace Presentation.Controllers {
    [Authorize(Roles = "seller")]
    public class CouponController : Controller {
        private readonly ILogger<CouponController> _logger;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;

        public CouponController(ILogger<CouponController> logger,
            ICouponService couponService,
            IProductService productService) {
            _logger = logger;
            _couponService = couponService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageCoupon(int page = 1, string? search = null, string? sort = null, string? direction = "asc",
            DateTime? startDateFrom = null, DateTime? startDateTo = null, int? maxUsageSearch = null, int? productIdSearch = null) {
            var sellerIdClaim = User.FindFirst("SellerId");
            if (sellerIdClaim == null)
                return Unauthorized();

            int sellerId = int.Parse(sellerIdClaim.Value ?? "0");
            if (sellerId == 0)
                return Unauthorized();

            int pageSize = 6;
            var coupons = await _couponService.GetAllCouponsBySellerAsync(sellerId);

            // search filters
            if (!string.IsNullOrEmpty(search))
                coupons = coupons.Where(c => (c.Code ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (startDateFrom.HasValue)
                coupons = coupons.Where(c => c.StartDate >= startDateFrom.Value).ToList();
            if (startDateTo.HasValue)
                coupons = coupons.Where(c => c.StartDate <= startDateTo.Value).ToList();
            if (maxUsageSearch.HasValue)
                coupons = coupons.Where(c => c.MaxUsage == maxUsageSearch.Value).ToList();
            if (productIdSearch.HasValue && productIdSearch.Value > 0)
                coupons = coupons.Where(c => c.ProductId == productIdSearch.Value).ToList();

            // sort filters
            if (!string.IsNullOrEmpty(sort)) {
                bool isDesc = direction?.ToLower() == "desc";
                coupons = sort switch {
                    "code" => isDesc ? coupons.OrderByDescending(c => c.Code).ToList() : coupons.OrderBy(c => c.Code).ToList(),
                    "discount" => isDesc ? coupons.OrderByDescending(c => c.DiscountPercent).ToList() : coupons.OrderBy(c => c.DiscountPercent).ToList(),
                    "startDate" => isDesc ? coupons.OrderByDescending(c => c.StartDate).ToList() : coupons.OrderBy(c => c.StartDate).ToList(),
                    "endDate" => isDesc ? coupons.OrderByDescending(c => c.EndDate).ToList() : coupons.OrderBy(c => c.EndDate).ToList(),
                    "maxUsage" => isDesc ? coupons.OrderByDescending(c => c.MaxUsage).ToList() : coupons.OrderBy(c => c.MaxUsage).ToList(),
                    "productId" => isDesc ? coupons.OrderByDescending(c => c.ProductId).ToList() : coupons.OrderBy(c => c.ProductId).ToList(),
                    _ => coupons
                };
            }

            var paginatedCoupons = coupons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int) Math.Ceiling((double) coupons.Count / pageSize);

            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = products.Where(p => p.SellerId == sellerId).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.Sort = sort;
            ViewBag.Direction = direction;

            ViewBag.StartDateFrom = startDateFrom;
            ViewBag.StartDateTo = startDateTo;
            ViewBag.MaxUsageSearch = maxUsageSearch;
            ViewBag.ProductIdSearch = productIdSearch;

            var model = new CouponVM {
                Coupons = coupons,
                TotalPages = totalPages,
                CouponsPage = paginatedCoupons
            };

            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"];

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon) {
            try {
                var sellerIdClaim = User.FindFirst("SellerId");
                int sellerId = int.Parse(sellerIdClaim?.Value ?? "0");
                if (sellerId == 0)
                    return Unauthorized();

                var product = await _productService.GetByIdAsync(coupon.ProductId ?? 0);
                if (product == null || product.SellerId != sellerId)
                    return BadRequest("Invalid product. It must belong to your store.");

                await _couponService.AddAsync(coupon, sellerId);
                TempData["Success"] = "Coupon created successfully.";
            }
            catch (ArgumentException ex) {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex) {
                TempData["Error"] = "An error occurred while creating coupon.";
                _logger.LogError(ex, "Create Coupon error");
            }

            return RedirectToAction(nameof(ManageCoupon));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Coupon coupon) {
            try {
                var sellerIdClaim = User.FindFirst("SellerId");
                int sellerId = int.Parse(sellerIdClaim?.Value ?? "0");
                if (sellerId == 0)
                    return Unauthorized();

                var product = await _productService.GetByIdAsync(coupon.ProductId ?? 0);
                if (product == null || product.SellerId != sellerId)
                    return BadRequest("Invalid product. It must belong to your store.");

                await _couponService.UpdateAsync(coupon, sellerId);
                TempData["Success"] = "Coupon updated successfully.";
            }
            catch (ArgumentException ex) {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex) {
                TempData["Error"] = "An error occurred while updating coupon.";
                _logger.LogError(ex, "Update Coupon error");
            }

            return RedirectToAction(nameof(ManageCoupon));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _couponService.DeleteAsync(id);
                TempData["Success"] = "Coupon deleted successfully.";
            }
            catch (ArgumentException ex) {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex) {
                TempData["Error"] = "An error occurred while deleting coupon.";
                _logger.LogError(ex, "Delete Coupon error");
            }

            return RedirectToAction(nameof(ManageCoupon));
        }
    }
}