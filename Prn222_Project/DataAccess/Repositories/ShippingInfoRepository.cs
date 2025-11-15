using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ShippingInfoRepository : IShippingInfoRepo
    {
        private readonly CloneEbayDbContext _context;

        public ShippingInfoRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShippingInfo shippingInfo)
        {
            await _context.ShippingInfos.AddAsync(shippingInfo);
            await _context.SaveChangesAsync();
        }

        public async Task<ShippingInfo?> GetByOrderIdAsync(int orderId)
        {
            return await _context.ShippingInfos.FirstOrDefaultAsync(s => s.OrderId == orderId);
        }

        public async Task UpdateAsync(ShippingInfo shippingInfo)
        {
            _context.ShippingInfos.Update(shippingInfo);
            await _context.SaveChangesAsync();
        }
        public async Task<ShippingInfo?> GetByTrackingCodeAsync(string trackingCode)
        {
            return await _context.ShippingInfos
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingCode);
        }
    }
}