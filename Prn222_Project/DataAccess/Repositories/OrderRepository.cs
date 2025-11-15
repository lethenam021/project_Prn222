using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using System;
using System.Linq;
using System.Text;

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
        // Số lượng đơn trong ngày của 1 seller
        public async Task<int> GetOrderQuantityAsync(int sellerId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            Console.WriteLine(sellerId);
            Console.WriteLine(date.ToString());

            return await _context.OrderTables
                .Where(o => o.OrderDate >= start && o.OrderDate < end)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
                .CountAsync();
        }

        // Doanh thu theo ngày của 1 seller (theo OrderTable)
        public async Task<decimal> GetRevenueAsync(int sellerId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            // trả về decimal? vì TotalPrice là decimal?
            var revenue = await _context.OrderTables
                .Where(o => o.OrderDate >= start && o.OrderDate < end)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
                .SumAsync(o => (decimal?)o.TotalPrice);   // ép về decimal?

            // nếu null (không có đơn) thì trả về 0
            return revenue ?? 0m;
        }
    }
}
