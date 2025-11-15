using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IOrderRepo
    {
        Task<OrderTable?> GetByIdAsync(int orderId);
        Task UpdateAsync(OrderTable order);
        Task<IEnumerable<OrderTable>> GetOrdersBySellerIdAsync(int sellerId);
        Task<bool> IsOrderOfSellerAsync(int orderId, int sellerId);
    }
}