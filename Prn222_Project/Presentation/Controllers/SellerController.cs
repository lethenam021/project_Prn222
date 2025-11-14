using BusinessLogic.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.ViewModel;

namespace Presentation.Controllers {
    public class SellerController : Controller {
        private readonly ILogger<SellerController> _logger;
        private readonly IStoreSevice _storeService;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SellerController(ILogger<SellerController> logger,
             IStoreSevice storeService,
             IProductService productService,
             ICouponService couponService) {
            _logger = logger;
            _storeService = storeService;
            _productService = productService;
            _couponService = couponService;
        }

        public async Task<IActionResult> ManagerStore() {
            var stores = await _storeService.GetAllStoresAsync();
            var model = new StoreVM {
                Stores = stores,

            };
            return View(model);
        }

        public async Task<IActionResult> ManagerProduct(int page = 1) {
            int pageSize = 6;

            var products = await _productService.GetAllProductsAsync();
            var paginatedProducts = products
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            var totalPages = (int) Math.Ceiling((double) products.Count / pageSize);

            var model = new ProductVM {
                Products = products,
                TotalPages = totalPages,
                ProductsPage = paginatedProducts,
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManagerCoupon(int page = 1) {
            // Giả sử lấy sellerId từ auth (adjust theo JWT/session)
            int sellerId = int.Parse(User.FindFirst("SellerId")?.Value ?? "0");
            if (sellerId == 0)
                return Unauthorized();

            int pageSize = 6;
            var coupons = await _couponService.GetAllCouponsBySellerAsync(sellerId);
            var paginatedCoupons = coupons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int) Math.Ceiling((double) coupons.Count / pageSize);

            var model = new CouponVM {
                Coupons = coupons,
                TotalPages = totalPages,
                CouponsPage = paginatedCoupons
            };

            ViewBag.CurrentPage = page;
            return View(model);
        }

        // CHANGE: POST Create
        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon) {
            int sellerId = int.Parse(User.FindFirst("SellerId")?.Value ?? "0");
            coupon.ProductId = 0; // Hoặc từ form, attach to product cụ thể
            await _couponService.AddAsync(coupon);
            return RedirectToAction(nameof(ManagerCoupon));
        }

        // CHANGE: POST Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Coupon coupon) {
            await _couponService.UpdateAsync(coupon);
            return RedirectToAction(nameof(ManagerCoupon));
        }

        // CHANGE: POST Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            await _couponService.DeleteAsync(id);
            return RedirectToAction(nameof(ManagerCoupon));
        }
    }
}
