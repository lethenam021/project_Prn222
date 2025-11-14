using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Pagination {
    public class PaginationRequest {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
