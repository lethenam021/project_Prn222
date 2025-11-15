using BusinessLogic.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.ViewModel;
using System.Security.Claims;

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

        // CHANGE: Sửa tên action từ ManagerCoupon sang ManageCoupon (GET), và các redirect tương ứng
        [HttpGet]
        public async Task<IActionResult> ManageCoupon(int page = 1, string? search = null, string? sort = null) {
            var sellerIdClaim = User.FindFirst("SellerId");
            if (sellerIdClaim == null)
                return Unauthorized();

            int sellerId = int.Parse(sellerIdClaim.Value ?? "0");
            if (sellerId == 0)
                return Unauthorized();

            int pageSize = 6;
            var coupons = await _couponService.GetAllCouponsBySellerAsync(sellerId);

            if (!string.IsNullOrEmpty(search))
                coupons = coupons.Where(c => (c.Code ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrEmpty(sort)) {
                coupons = sort switch {
                    "code" => coupons.OrderBy(c => c.Code).ToList(),
                    "discount" => coupons.OrderByDescending(c => c.DiscountPercent).ToList(),
                    "endDate" => coupons.OrderByDescending(c => c.EndDate).ToList(),
                    _ => coupons
                };
            }

            var paginatedCoupons = coupons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int) Math.Ceiling((double) coupons.Count / pageSize);

            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = products.Where(p => p.SellerId == sellerId).ToList();
            ViewBag.CurrentPage = page;

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
            if (!ModelState.IsValid) {
                TempData["Error"] = "Invalid coupon data.";
                return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
            }

            var sellerIdClaim = User.FindFirst("SellerId");
            int sellerId = int.Parse(sellerIdClaim?.Value ?? "0");
            if (sellerId == 0)
                return Unauthorized();

            if (coupon.ProductId == null || coupon.ProductId == 0) {
                TempData["Error"] = "Product is required.";
                return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
            }

            await _couponService.AddAsync(coupon);
            TempData["Success"] = "Coupon created successfully.";
            return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon) {
            if (!ModelState.IsValid) {
                TempData["Error"] = "Invalid coupon data.";
                return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
            }

            await _couponService.UpdateAsync(coupon);
            TempData["Success"] = "Coupon updated successfully.";
            return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            await _couponService.DeleteAsync(id);
            TempData["Success"] = "Coupon deleted successfully.";
            return RedirectToAction(nameof(ManageCoupon)); // CHANGE: Sửa redirect sang ManageCoupon
        }
    }
}