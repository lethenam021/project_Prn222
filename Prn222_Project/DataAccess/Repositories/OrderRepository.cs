using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class OrderRepository : IOrderRepo
    {
        private readonly CloneEbayDbContext _context;

        public OrderRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<OrderTable?> GetByIdAsync(int orderId)
        {
            return await _context.OrderTables
                .Include(o => o.Address)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateAsync(OrderTable order)
        {
            _context.OrderTables.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderTable>> GetOrdersBySellerIdAsync(int sellerId)
        {
            return await _context.OrderTables
                .AsNoTracking()                         
                .Include(o => o.Address)                
                .Where(o => o.OrderItems.Any(
                    oi => oi.Product != null && oi.Product.SellerId == sellerId))
                .OrderByDescending(o => o.OrderDate)
                .Take(50)                             
                .ToListAsync();
        }

        public async Task<bool> IsOrderOfSellerAsync(int orderId, int sellerId)
        {
            return await _context.OrderTables
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .AnyAsync(o => o.OrderItems.Any(
                    oi => oi.Product != null && oi.Product.SellerId == sellerId));
        }

    }
}