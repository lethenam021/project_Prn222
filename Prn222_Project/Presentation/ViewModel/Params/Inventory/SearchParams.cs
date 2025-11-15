using Common.Enums;
using Presentation.ViewModel.Params.Pagination;

namespace Presentation.ViewModel.Params.Inventory {
    public class SearchParams {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public StockStatusEnum StockStatus { get; set; } = StockStatusEnum.All;
        public PaginationParams Pagination { get; set; } = new PaginationParams { PageIndex = 1, PageSize = 6 };

    }
}
