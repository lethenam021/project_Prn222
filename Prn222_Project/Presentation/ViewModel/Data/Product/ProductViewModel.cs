using Presentation.ViewModel.Data.Category;

namespace Presentation.ViewModel.Data.Product {
    public class ProductViewModel {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? ImageUrl { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
