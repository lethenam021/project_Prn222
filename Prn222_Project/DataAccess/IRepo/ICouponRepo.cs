using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public interface ICouponRepo {
        Task<List<Coupon>> GetAllCouponsBySellerAsync(int sellerId);
        Task<Coupon?> GetByIdAsync(int id);
        Task AddAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(int id);
    }
}