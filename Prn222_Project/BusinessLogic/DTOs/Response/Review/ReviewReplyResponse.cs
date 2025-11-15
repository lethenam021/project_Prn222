using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response.Review {
    public class ReviewReplyResponse {
        public string ReplyMessage { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
