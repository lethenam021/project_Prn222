using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface ICouponService {
        Task<List<Coupon>> GetAllCouponsBySellerAsync(int sellerId);
        Task<Coupon?> GetByIdAsync(int id);
        Task AddAsync(Coupon coupon, int sellerId);
        Task UpdateAsync(Coupon coupon, int sellerId);
        Task DeleteAsync(int id);
    }
}