using DataAccess.Models;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IShippingInfoRepo
    {
        Task AddAsync(ShippingInfo shippingInfo);
        Task<ShippingInfo?> GetByOrderIdAsync(int orderId);
        Task UpdateAsync(ShippingInfo shippingInfo);
        Task<ShippingInfo?> GetByTrackingCodeAsync(string trackingCode);
    }
}