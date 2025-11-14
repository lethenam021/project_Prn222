using BusinessLogic.DTOs.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response.Review {
    public class ReviewSummaryResponse {
        public ProductResponse Product { get; set; } = null!;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
