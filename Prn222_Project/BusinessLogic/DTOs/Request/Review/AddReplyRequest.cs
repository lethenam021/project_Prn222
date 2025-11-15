using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Review {
    public class AddReplyRequest {
        // SellerId sẽ được lấy từ Claims trong Controller
        public int SellerId { get; set; }

        // Đây là 'reviewId' từ bảng dbo.ReviewReply
        public int ReviewId { get; set; }

        // Đây là 'replyMessage'
        public string ReplyMessage { get; set; } = null!;
    }
}
