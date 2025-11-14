using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers {
    public class PagedResult<T> {
        // Danh sách các mục trên trang hiện tại
        public List<T> Items { get; set; } = new List<T>();

        // Tổng số mục (tổng số record trong database)
        public int TotalRecord { get; set; }

        // Trang hiện tại (ví dụ: 1, 2, 3...)
        public int PageIndex { get; set; }

        // Kích thước trang (ví dụ: 10 mục/trang)
        public int PageSize { get; set; }

        // --- Các thuộc tính (properties) tự tính toán ---

        // Tổng số trang
        public int TotalPages => (int)Math.Ceiling(TotalRecord / (double)PageSize);

        // Có trang trước đó không?
        public bool HasPreviousPage => PageIndex > 1;

        // Có trang kế tiếp không?
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
