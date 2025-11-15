using Presentation.ViewModel.Params.Pagination;

namespace Presentation.ViewModel.Params.ProductReviews {
    public class SearchParams {
        public int SellerId { get; set; }
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public PaginationParams Pagination { get; set; } = new PaginationParams { PageIndex = 1, PageSize = 6 };
    }
}
