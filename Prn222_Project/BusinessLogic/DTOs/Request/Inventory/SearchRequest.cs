using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Inventory {
    public class SearchRequest {
        public int SellerId { get; set; }
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public StockStatusEnum StockStatus { get; set; }
    }
}
    