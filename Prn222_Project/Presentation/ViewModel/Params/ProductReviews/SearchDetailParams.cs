using Presentation.ViewModel.Params.Pagination;

namespace Presentation.ViewModel.Params.ProductReviews {
    public class SearchDetailParams {
        public int SellerId { get; set; }
        public int ProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public PaginationParams Pagination { get; set; } = new PaginationParams { PageIndex = 1, PageSize = 6 };
    }
}
