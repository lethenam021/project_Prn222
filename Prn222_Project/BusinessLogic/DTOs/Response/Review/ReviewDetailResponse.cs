using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response.Review {
    public class ReviewDetailResponse {
        public int Id { get; set; }
        public string Reviewer { get; set; } = null!;
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
