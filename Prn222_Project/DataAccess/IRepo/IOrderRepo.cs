using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.IRepo
{
    public interface IOrderRepo
    {
        Task<int> GetOrderQuantityAsync(int sellerId, DateTime date);
        Task<decimal> GetRevenueAsync(int sellerId, DateTime date);
    }
}
