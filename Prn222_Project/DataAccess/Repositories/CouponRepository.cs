using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class CouponRepository : ICouponRepo {
        private readonly CloneEbayDbContext _context;

        public CouponRepository(CloneEbayDbContext context) {
            _context = context;
        }

        public async Task<List<Coupon>> GetAllCouponsBySellerAsync(int sellerId) {
            return await _context.Coupons
                                .Include(c => c.Product)
                                .Where(c => c.Product.SellerId == sellerId)
                                .ToListAsync();
        }

        public async Task<Coupon?> GetByIdAsync(int id) {
            return await _context.Coupons
                                .Include(c => c.Product)
                                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Coupon coupon) {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon) {
            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null) {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
        }
    }
}