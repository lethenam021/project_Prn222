using BusinessLogic.DTOs.Request.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Review {
    public class SearchRequest {
        public int SellerId { get; set; }
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PaginationRequest? Pagination { get; set; } 
    }
}
