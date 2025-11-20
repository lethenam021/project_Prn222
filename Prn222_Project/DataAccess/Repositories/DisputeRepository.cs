using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class DisputeRepository : IDisputeRepo
    {
        private readonly CloneEbayDbContext _context;

        public DisputeRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả khiếu nại của các đơn hàng thuộc 1 seller
        public async Task<IEnumerable<Dispute>> GetDisputesForSellerAsync(int sellerId)
        {
            return await _context.Disputes
                .Include(d => d.RaisedByNavigation)               
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Where(d => d.Order.OrderItems
                    .Any(oi => oi.Product.SellerId == sellerId))
                .ToListAsync();
        }


        // Lấy chi tiết 1 khiếu nại
        public async Task<Dispute?> GetByIdAsync(int id)
        {
            return await _context.Disputes
                .Include(d => d.RaisedByNavigation)        
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(d => d.Id == id);
        }


        // Tạo mới khiếu nại (buyer hoặc seller tạo)
        public async Task AddAsync(Dispute dispute)
        {
            _context.Disputes.Add(dispute);
            await _context.SaveChangesAsync();
        }

        // Cập nhật khiếu nại (status, resolution, ...)
        public async Task UpdateAsync(Dispute dispute)
        {
            _context.Disputes.Update(dispute);
            await _context.SaveChangesAsync();
        }
    }
}
