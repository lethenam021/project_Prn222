using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims; // Sẽ cần để lấy ID

namespace Presentation.Controllers
{
    // [Authorize(Roles = "Seller")] // Bạn nên thêm cái này để bảo mật
    public class SellerOrderController : Controller
    {
        // Controller chi lam viec voi Service
        private readonly ISellerOrderService _orderService;

        public SellerOrderController(ISellerOrderService orderService)
        {
            _orderService = orderService;
        }

        
        private int GetCurrentUserId()
        {
            // TODO: Thay bằng logic lấy ID thật
            // Vi du:
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            // int.TryParse(userIdClaim.Value, out int userId);
            // return userId;

            return 1; // TẠM THỜI GIẢ LẬP ID Seller = 1 DE TEST
        }

       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sellerId = GetCurrentUserId();
            var orderDtos = await _orderService.GetOrdersBySellerIdAsync(sellerId);
            return View(orderDtos); // Truyen DTO sang View
        }

        // POST: /SellerOrder/ConfirmAndPrintLabel
        // Nhan request tu nut "Xac nhan & In phieu"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAndPrintLabel(int orderId)
        {
            var sellerId = GetCurrentUserId();

            // Goi Service de xu ly nghiep vu (goi 17track, cap nhat DB)
            var labelDto = await _orderService.ConfirmOrderAndCreateLabelAsync(orderId, sellerId);

            if (labelDto == null)
            {
                // Neu Service tra ve null (vd: don da xu ly, khong tim thay)
                TempData["Error"] = "Không thể xác nhận đơn hàng này.";
                return RedirectToAction("Index");
            }

            // Tra về View "In phiếu" (gia lap)
            return View("PrintLabel", labelDto);
        }

        // POST: /SellerOrder/UpdateOrderStatus
        // Nhan request tu 2 nut "Giao thanh cong" / "Giao that bai"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            var sellerId = GetCurrentUserId();
            var success = await _orderService.UpdateOrderStatusAsync(orderId, sellerId, newStatus);

            if (success)
            {
                TempData["Success"] = "Cập nhật trạng thái thủ công thành công.";
            }
            else
            {
                TempData["Error"] = "Cập nhật thất bại.";
            }

            return RedirectToAction("Index");
        }
    }
}