using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IPaymentRepo
    {
        Task<decimal> GetRevenueAsync(int sellerId, DateTime date);
    }
}
