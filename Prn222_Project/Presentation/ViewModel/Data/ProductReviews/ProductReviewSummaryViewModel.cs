using Presentation.ViewModel.Data.Product;

namespace Presentation.ViewModel.Data.ProductReviews {
    public class ProductReviewSummaryViewModel {
        public int StartIndex { get; set; }
        public ProductViewModel Product { get; set; } // Thông tin sản phẩm
        public double AverageRating { get; set; }    // Rating trung bình
        public int TotalReviews { get; set; }     // Tổng số reviews

    }
}
