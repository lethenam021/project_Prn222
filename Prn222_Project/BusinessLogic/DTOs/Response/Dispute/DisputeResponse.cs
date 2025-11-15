using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace BusinessLogic.DTOs.Response.Dispute
{
    public class DisputeResponse
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        // Người khiếu nại (Disputer)
        public int DisputerId { get; set; }
        public string? DisputerName { get; set; }
        public string? DisputerEmail { get; set; }

        // Thông tin đơn hàng
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }

        // Sản phẩm trong đơn
        public string ProductTitles { get; set; } = string.Empty;

        // Nội dung khiếu nại
        public string? Description { get; set; }

        // Trạng thái dispute (Pending / Accepted / Rejected)
        public string Status { get; set; } = "Pending";

        // Resolution (100% refund / partial refund / lý do reject)
        public string? Resolution { get; set; }
    }
}
