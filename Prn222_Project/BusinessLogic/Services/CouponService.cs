using BusinessLogic.Interface;
using DataAccess.IRepo;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class CouponService : ICouponService {
        private readonly ICouponRepo _repo;

        public CouponService(ICouponRepo repo) {
            _repo = repo;
        }

        public async Task<List<DataAccess.Models.Coupon>> GetAllCouponsBySellerAsync(int sellerId)
            => await _repo.GetAllCouponsBySellerAsync(sellerId);

        public async Task<DataAccess.Models.Coupon?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task AddAsync(DataAccess.Models.Coupon coupon) {
            if (string.IsNullOrEmpty(coupon.Code) || coupon.DiscountPercent < 0 || coupon.DiscountPercent > 100)
                throw new ArgumentException("Invalid coupon data");
            if (coupon.StartDate >= coupon.EndDate)
                throw new ArgumentException("Start date must be before end date");
            await _repo.AddAsync(coupon);
        }

        public async Task UpdateAsync(DataAccess.Models.Coupon coupon) {
            if (string.IsNullOrEmpty(coupon.Code) || coupon.DiscountPercent < 0 || coupon.DiscountPercent > 100)
                throw new ArgumentException("Invalid coupon data");
            await _repo.UpdateAsync(coupon);
        }

        public async Task DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}