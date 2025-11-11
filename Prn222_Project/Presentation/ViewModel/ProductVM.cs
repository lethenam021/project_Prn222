using DataAccess.Models;
namespace Presentation.ViewModel
{
    public class ProductVM
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> ProductsPage { get; set; } = new List<Product>();

    }
}
