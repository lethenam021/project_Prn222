using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BusinessLogic.DTOs.Request.Dispute
{
    public class SearchRequest
    {
        // Tên sản phẩm cần tìm (có thể null / rỗng)
        public string? ProductName { get; set; }

        // Lọc theo ngày đặt đơn (OrderDate) từ ngày ...
        public DateTime? FromDate { get; set; }

        // ... đến ngày
        public DateTime? ToDate { get; set; }
    }
}

