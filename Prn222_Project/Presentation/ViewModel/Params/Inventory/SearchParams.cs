using Common.Enums;

namespace Presentation.ViewModel.Params.Inventory {
    public class SearchParams {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public StockStatusEnum StockStatus { get; set; } = StockStatusEnum.All;
    }
}
