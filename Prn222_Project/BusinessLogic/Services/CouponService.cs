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

        public async Task AddAsync(DataAccess.Models.Coupon coupon, int sellerId) {
            if (string.IsNullOrEmpty(coupon.Code?.Trim()))
                throw new ArgumentException("Code is required and cannot be empty.");
            if (coupon.DiscountPercent < 0 || coupon.DiscountPercent > 100)
                throw new ArgumentException("Discount percent must be between 0 and 100.");
            if (coupon.StartDate >= coupon.EndDate)
                throw new ArgumentException("Start date must be before end date.");
            if (coupon.MaxUsage <= 0)
                throw new ArgumentException("Max usage must be positive integer.");
            if (coupon.ProductId <= 0)
                throw new ArgumentException("Valid product ID is required.");

            if (sellerId <= 0)
                throw new ArgumentException("Invalid seller ID.");

            var sellerCoupons = await _repo.GetAllCouponsBySellerAsync(sellerId);
            if (sellerCoupons.Any(c => c.Code.Trim().Equals(coupon.Code.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Coupon code must be unique for this seller.");

            await _repo.AddAsync(coupon);
        }

        public async Task UpdateAsync(DataAccess.Models.Coupon coupon, int sellerId) {
            if (string.IsNullOrEmpty(coupon.Code?.Trim()))
                throw new ArgumentException("Code is required and cannot be empty.");
            if (coupon.DiscountPercent < 0 || coupon.DiscountPercent > 100)
                throw new ArgumentException("Discount percent must be between 0 and 100.");
            if (coupon.StartDate >= coupon.EndDate)
                throw new ArgumentException("Start date must be before end date.");
            if (coupon.MaxUsage <= 0)
                throw new ArgumentException("Max usage must be positive integer.");
            if (coupon.ProductId <= 0)
                throw new ArgumentException("Valid product ID is required.");

            var existing = await _repo.GetByIdAsync(coupon.Id);
            if (existing == null)
                throw new ArgumentException("Coupon not found.");

            if (sellerId <= 0)
                throw new ArgumentException("Invalid seller ID.");

            var sellerCoupons = await _repo.GetAllCouponsBySellerAsync(sellerId);
            if (sellerCoupons.Any(c => c.Code.Trim().Equals(coupon.Code.Trim(), StringComparison.OrdinalIgnoreCase) && c.Id != coupon.Id))
                throw new ArgumentException("Coupon code must be unique for this seller.");

            existing.Code = coupon.Code;
            existing.DiscountPercent = coupon.DiscountPercent;
            existing.StartDate = coupon.StartDate;
            existing.EndDate = coupon.EndDate;
            existing.MaxUsage = coupon.MaxUsage;
            existing.ProductId = coupon.ProductId;

            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id) {
            var coupon = await _repo.GetByIdAsync(id);
            if (coupon == null)
                throw new ArgumentException("Coupon not found.");
            await _repo.DeleteAsync(id);
        }
    }
}