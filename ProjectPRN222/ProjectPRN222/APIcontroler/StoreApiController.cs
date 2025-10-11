using Microsoft.AspNetCore.Mvc;
using ProjectPRN222.Models;
using ProjectPRN222.Services;
using ProjectPRN222.DTO;

namespace ProjectPRN222.APIcontroler
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreApiController : ControllerBase
    {
        private readonly StoreService _svc;
        public StoreApiController(StoreService svc) => _svc = svc;

        [HttpGet("GetAllStorelist")]
        public async Task<IActionResult> GetAllStorelist()
        {
            var stores = _svc.InitializeAsync(); // <-- await
            return Ok(stores);
        }
        [HttpGet("GetUserByStore")]
        public async Task<IActionResult> GetUserByStore(int id)
        {
            var user = await _svc.GetUserByStoreIdAsync(id);
            return Ok(user);
        }
        [HttpGet("GetProductByUser")]
        public async Task<IActionResult> GetProductByUser(int id)
        {
            var products = await _svc.GetProductAsync(id);
            return Ok(products);
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddStore(StoreDTO dto)
        {
            var products = new StoreDTO
            {
                sellerId = dto.sellerId,
                storeName = dto.storeName,
                description = dto.description,
                bannerImageURL = dto.bannerImageURL
            };
            return Ok(products);
        }



    }

}
