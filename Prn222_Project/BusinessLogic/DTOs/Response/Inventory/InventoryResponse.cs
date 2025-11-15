using BusinessLogic.DTOs.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response.Inventory {
    public class InventoryResponse {
        public int Id { get; set; }
        public ProductResponse Product { get; set; }
        public int Quantity { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
