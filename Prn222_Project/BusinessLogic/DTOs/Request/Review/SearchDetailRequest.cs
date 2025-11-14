using BusinessLogic.DTOs.Request.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Review {
    public class SearchDetailRequest {
        public int SellerId { get; set; }
        public int ProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public PaginationRequest? Pagination { get; set; }
    }
}
