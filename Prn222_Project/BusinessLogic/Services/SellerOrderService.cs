using BusinessLogic.DTOs.Response.Order;
using BusinessLogic.Interface;
using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SellerOrderService : ISellerOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IShippingInfoRepo _shippingInfoRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _17trackKey;

        public SellerOrderService(IOrderRepo orderRepo,
                                IShippingInfoRepo shippingInfoRepo,
                                IConfiguration configuration,
                                IHttpClientFactory httpClientFactory)
        {
            _orderRepo = orderRepo;
            _shippingInfoRepo = shippingInfoRepo;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _17trackKey = _configuration["17track:ApiKey"];

            
            Console.WriteLine($"--- 17TRACK KEY DA DOC: '{_17trackKey}' ---");
        }

        public async Task<IEnumerable<SellerOrderDto>> GetOrdersBySellerIdAsync(int sellerId)
        {
            var orders = await _orderRepo.GetOrdersBySellerIdAsync(sellerId);

            return orders.Select(o => new SellerOrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                BuyerName = o.Address?.FullName,
                ShippingAddress = $"{o.Address?.Street}, {o.Address?.City}, {o.Address?.State}"
            });
        }

        public async Task<ShippingLabelDto?> ConfirmOrderAndCreateLabelAsync(int orderId, int sellerId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null || order.Status != "Pending") return null;
            if (!await IsOrderOfSeller(orderId, sellerId)) return null;

            order.Status = "Shipped";
            await _orderRepo.UpdateAsync(order);

            
            var randomDigits = new Random().Next(100000000, 999999999).ToString();

            var shippingInfo = new ShippingInfo
            {
                
                Carrier = "ChinaPost",

                
                TrackingNumber = "RR" + randomDigits + "CN",

                Status = "Submitted",
                
            };

           
            Console.WriteLine($"DEBUG: Sending Reg Request - Number: {shippingInfo.TrackingNumber}");
            // ...

            await _shippingInfoRepo.AddAsync(shippingInfo); // Giả sử hàm này tự SaveChanges

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("17token", _17trackKey);

                var payload = new[] {
                    new {
                        number = shippingInfo.TrackingNumber,
                        carrier = ""
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // --- SỬA LẠI TỪ ĐÂY ---

                // 1. Gửi request và nhận về response
                var response = await client.PostAsync("https://api.17track.net/track/v2.4/register", content);

                // 2. Kiểm tra xem response có thành công không (code 2xx)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("--- 17TRACK API: Đăng ký thành công! ---");
                }
                else
                {
                    // 3. Nếu thất bại (401, 500, ...), ĐỌC LỖI mà 17track trả về
                    string errorBody = await response.Content.ReadAsStringAsync();

                    // 4. In lỗi chi tiết này ra console
                    Console.WriteLine($"--- LOI 17TRACK API [{response.StatusCode}] ---");
                    Console.WriteLine(errorBody); // <--- ĐÂY LÀ THỨ QUAN TRỌNG NHẤT

                    // 5. Đã thất bại, không trả về DTO
                    return null;
                }

                
            }
            catch (Exception ex)
            {
                // Catch này chỉ bắt lỗi mạng (vd: không có internet)
                Console.WriteLine($"LOI KHI DANG KY 17TRACK (Exception): {ex.Message}");
                return null;
            }

            return new ShippingLabelDto
            {
                OrderId = order.Id,
                Carrier = shippingInfo.Carrier,
                TrackingNumber = shippingInfo.TrackingNumber,
                EstimatedArrival = shippingInfo.EstimatedArrival,
                BuyerName = order.Address?.FullName,
                FullAddress = $"{order.Address?.Street}, {order.Address?.City}, {order.Address?.State}",
                Phone = order.Address?.Phone
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int sellerId, string newStatus)
        {
            if (newStatus != "Delivered" && newStatus != "Failed") return false;
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.Status != "Shipped") return false;
            if (!await IsOrderOfSeller(orderId, sellerId)) return false;

            order.Status = newStatus;
            await _orderRepo.UpdateAsync(order);

            var shippingInfo = await _shippingInfoRepo.GetByOrderIdAsync(orderId);
            if (shippingInfo != null)
            {
                shippingInfo.Status = newStatus;
                await _shippingInfoRepo.UpdateAsync(shippingInfo);
            }
            return true;
        }

        private Task<bool> IsOrderOfSeller(int orderId, int sellerId)
        {
            return _orderRepo.IsOrderOfSellerAsync(orderId, sellerId);
        }
    }
}