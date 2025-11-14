using DataAccess.Models;
using System.Collections.Generic;

namespace Presentation.ViewModel {
    public class CouponVM {
        public List<Coupon> Coupons { get; set; } = new();
        public int TotalPages { get; set; }
        public List<Coupon> CouponsPage { get; set; } = new();
    }
}