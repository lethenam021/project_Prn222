using Common.Helpers;
using DataAccess.IRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class ReviewRepository : IReviewRepo {
        private readonly CloneEbayDbContext _context;

        public ReviewRepository(CloneEbayDbContext context) {
            _context = context;
        }

        public async Task<PagedResult<Review>> GetReviewsAsync(int sellerId, string? productName, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize) {
            var query = _context.Reviews.Include(r => r.Reviewer).Include(r => r.Product).ThenInclude(p => p.Category).AsNoTracking().AsQueryable();

            query = query.Where(r => r.Product != null && r.Product.SellerId == sellerId);

            if (!string.IsNullOrEmpty(productName)) {
                query = query.Where(r => r.Product != null && r.Product.Title!.Contains(productName));
            }

            if (fromDate.HasValue) {
                query = query.Where(r => r.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue) {
                query = query.Where(r => r.CreatedAt <= toDate.Value);
            }

            var totalRecord = await query.CountAsync();

            var items = await query.OrderBy(r => r.Id).Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize).ToListAsync();

            return new PagedResult<Review> {
                Items = items,
                TotalRecord = totalRecord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
