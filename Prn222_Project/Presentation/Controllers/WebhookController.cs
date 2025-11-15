using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IShippingInfoRepo _shippingInfoRepo;

        public WebhookController(IOrderRepo orderRepo, IShippingInfoRepo shippingInfoRepo)
        {
            _orderRepo = orderRepo;
            _shippingInfoRepo = shippingInfoRepo;
        }

        [HttpPost("17track")]
        public async Task<IActionResult> Handle17TrackWebhook()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Console.WriteLine(">>> 17TRACK WEBHOOK RAW BODY:");
            Console.WriteLine(requestBody);

            try
            {
                using var jsonDoc = JsonDocument.Parse(requestBody);
                var root = jsonDoc.RootElement;

                // 1. Thử lấy data
                if (!root.TryGetProperty("data", out var dataElement) ||
                    dataElement.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Webhook: no 'data' property, skip updating.");
                    return Ok();
                }

                // --- BẮT ĐẦU SỬA LỖI ---

                // 2. Lấy tracking number (data.number)
                if (!dataElement.TryGetProperty("number", out var numberProp) ||
                    numberProp.ValueKind != JsonValueKind.String)
                {
                    Console.WriteLine("Webhook: no 'data.number' field, skip updating.");
                    return Ok();
                }
                var trackingNumber = numberProp.GetString();

                // 3. Lấy status (data.track_info.latest_status.status)
                if (!dataElement.TryGetProperty("track_info", out var trackInfoProp) ||
                    trackInfoProp.ValueKind != JsonValueKind.Object ||
                    !trackInfoProp.TryGetProperty("latest_status", out var latestStatusProp) ||
                    latestStatusProp.ValueKind != JsonValueKind.Object ||
                    !latestStatusProp.TryGetProperty("status", out var statusProp) ||
                    statusProp.ValueKind != JsonValueKind.String)
                {
                    Console.WriteLine("Webhook: no 'data.track_info.latest_status.status', skip updating.");
                    return Ok();
                }
                var newApiStatus = statusProp.GetString()?.ToLowerInvariant();

                // --- KẾT THÚC SỬA LỖI ---

                if (string.IsNullOrEmpty(trackingNumber) || string.IsNullOrEmpty(newApiStatus))
                {
                    Console.WriteLine("Webhook: trackingNumber or status empty, skip updating.");
                    return Ok();
                }

                // 4. Tìm ShippingInfo theo trackingNumber
                var shippingInfo = await _shippingInfoRepo.GetByTrackingCodeAsync(trackingNumber);
                if (shippingInfo == null || !shippingInfo.OrderId.HasValue)
                {
                    // LƯU Ý: Mã "1Z2617V10397725789" này có thể không có trong DB của bạn
                    // vì nó không phải mã "122816215025810" bạn dùng lúc trước
                    Console.WriteLine($"Webhook: no ShippingInfo or OrderId for tracking {trackingNumber}.");
                    return Ok();
                }

                var order = await _orderRepo.GetByIdAsync(shippingInfo.OrderId.Value);
                if (order == null)
                {
                    Console.WriteLine($"Webhook: order {shippingInfo.OrderId.Value} not found.");
                    return Ok();
                }

                // Chỉ KHÔNG auto-update nếu đơn đã ở trạng thái cuối cùng
                if (string.Equals(order.Status, "Completed", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(order.Status, "Failed", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Webhook: order {order.Id} status = {order.Status}, already final, skip auto-update.");
                    return Ok();
                }

                // 5. Map status 17TRACK -> status local
                string newLocalStatus = null;
                if (newApiStatus == "delivered")
                    newLocalStatus = "Completed";   // hoặc "Delivered" tuỳ bạn đặt trong DB
                else if (newApiStatus == "undelivered" || newApiStatus == "deliveryfailure")
                    newLocalStatus = "Failed";      // Giao thất bại
                else if (newApiStatus == "intransit" || newApiStatus == "outfordelivery" || newApiStatus == "pickup")
                    newLocalStatus = "Shipping";

                if (newLocalStatus != null && !string.Equals(order.Status, newLocalStatus, StringComparison.OrdinalIgnoreCase))
                {
                    order.Status = newLocalStatus;
                    shippingInfo.Status = newLocalStatus; // Cập nhật cả shippingInfo
                    await _orderRepo.UpdateAsync(order);
                    await _shippingInfoRepo.UpdateAsync(shippingInfo);

                    Console.WriteLine($"Webhook: updated order {order.Id} to {newLocalStatus}");
                }
                else
                {
                    Console.WriteLine($"Webhook: No status change needed for order {order.Id} (API: {newApiStatus}, Local: {order.Status})");
                }

                return Ok();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Webhook: JSON parse error: {ex.Message}");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook: unexpected error: {ex.Message}");
                return Ok();
            }
        }
    }
}