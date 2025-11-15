using BusinessLogic.DTOs.Response.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface ISellerOrderService
    {
        Task<IEnumerable<SellerOrderDto>> GetOrdersBySellerIdAsync(int sellerId);

        Task<ShippingLabelDto?> ConfirmOrderAndCreateLabelAsync(int orderId, int sellerId);
        
        Task<bool> UpdateOrderStatusAsync(int orderId, int sellerId, string newStatus);
    }
}