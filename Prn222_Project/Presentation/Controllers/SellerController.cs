using BusinessLogic.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.ViewModel;

namespace Presentation.Controllers
{
    public class SellerController : Controller
    {
        private readonly ILogger<SellerController> _logger; 
        private readonly IStoreSevice _storeService;
        private readonly IProductService _productService;

        public SellerController(ILogger<SellerController> logger,
             IStoreSevice storeService,
             IProductService productService)
        {
            _logger = logger;
            _storeService = storeService;
            _productService = productService;
        }
        public  async Task<IActionResult> ManagerStore()
        {
            var stores = await _storeService.GetAllStoresAsync();
            var model = new StoreVM
            {
                Stores = stores,

            };
            return View(model);
        }
        public async Task<IActionResult> ManagerProduct(int page = 1)
        {
            int pageSize = 6;

            var products = await _productService.GetAllProductsAsync();
            var paginatedProducts = products
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            var totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

            var model = new ProductVM
            {
                Products = products,
                TotalPages = totalPages,
                ProductsPage = paginatedProducts,
            };
            return View(model);
        }
    }
}
