using Azure.Core;
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

        public async Task<PagedResult<Review>> GetReviewDetailsAsync(int sellerId, int productId, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize) {
            var query = _context.Reviews
                                .Include(r => r.Reviewer)
                                .Include(r => r.Product) // Cần để lấy ProductId
                                .AsNoTracking()
                                .AsQueryable();

            // Filter chính
            query = query.Where(r => r.Product != null && r.Product.SellerId == sellerId);

            query = query.Where(r => r.Product!.Id == productId);

            if (fromDate.HasValue) {
                query = query.Where(r => r.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue) {
                query = query.Where(r => r.CreatedAt <= toDate.Value);
            }

            var totalRecord = await query.CountAsync();

            var items = await query.OrderByDescending(r => r.CreatedAt) // Mới nhất lên trên
                                   .Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<Review> {
                Items = items,
                TotalRecord = totalRecord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<RepoReviewSummary>> GetReviewSummariesAsync(int sellerId, string? productName, int? categoryId, int pageIndex, int pageSize) {
            var query = _context.Reviews
                                .Include(r => r.Product)
                                .ThenInclude(p => p.Category)
                                .AsNoTracking()
                                .AsQueryable();

            // Filter (Giữ nguyên)
            query = query.Where(r => r.Product != null && r.Product.SellerId == sellerId);

            if (categoryId.HasValue) {
                query = query.Where(r => r.Product != null && r.Product.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(productName)) {
                query = query.Where(r => r.Product != null && r.Product.Title!.Contains(productName));
            }

            // (FIX) 1. Grouping VÀ Projecting (Tính toán)
            // Tính toán Count và Average ngay trong SQL
            var aggregatedQuery = query.GroupBy(r => r.Product) // Group theo Product
                                       .Select(g => new RepoReviewSummary {
                                           Product = g.Key,
                                           TotalReviews = g.Count(),
                                           AverageRating = g.Average(r => r.Rating) ?? 0
                                       });

            // (FIX) 2. Order by (Sắp xếp)
            // Sắp xếp theo kết quả đã tính toán (TotalReviews)
            var orderedQuery = aggregatedQuery.OrderByDescending(x => x.TotalReviews);

            // 3. Paging (Phân trang)
            var totalRecord = await orderedQuery.CountAsync();

            var items = await orderedQuery
                                   .Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<RepoReviewSummary> {
                Items = items,
                TotalRecord = totalRecord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
