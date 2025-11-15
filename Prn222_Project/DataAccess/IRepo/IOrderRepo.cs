using DataAccess.Models;
using System.Threading.Tasks;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Models;

namespace DataAccess.IRepo
{
    public interface IOrderRepo
    {
        Task<OrderTable?> GetByIdAsync(int orderId);
        Task UpdateAsync(OrderTable order);
        Task<IEnumerable<OrderTable>> GetOrdersBySellerIdAsync(int sellerId);
        Task<bool> IsOrderOfSellerAsync(int orderId, int sellerId);
        Task<int> GetOrderQuantityAsync(int sellerId, DateTime date);
        Task<decimal> GetRevenueAsync(int sellerId, DateTime date);
    }
}
