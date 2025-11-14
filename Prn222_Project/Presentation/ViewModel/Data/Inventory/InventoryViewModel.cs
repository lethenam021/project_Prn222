using Presentation.ViewModel.Data.Product;

namespace Presentation.ViewModel.Data.Inventory {
    public class InventoryViewModel {
        public int Id { get; set; }
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
