using Presentation.ViewModel.Data.Product;

namespace Presentation.ViewModel.Data.ProductReviews {
    public class ProductReviewsViewModel {
        public int Id { get; set; }
        public int StartIndex { get; set; }
        public ProductViewModel Product { get; set; }
        public string Reviewer { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
