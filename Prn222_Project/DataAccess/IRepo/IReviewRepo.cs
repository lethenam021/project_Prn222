using Common.Helpers;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public class RepoReviewSummary {
        public Product Product { get; set; } = null!;
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }

    public interface IReviewRepo {
        // (Sửa phương thức cũ)
        // Phương thức Cấp 2: Lấy chi tiết review cho 1 sản phẩm
        Task<PagedResult<Review>> GetReviewDetailsAsync(int sellerId, int productId, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize);

        // (Thêm phương thức mới)
        // Phương thức Cấp 1: Lấy tóm tắt (group by) các sản phẩm
        Task<PagedResult<RepoReviewSummary>> GetReviewSummariesAsync(int sellerId, string? productName, int? categoryId, int pageIndex, int pageSize);
    }
}
