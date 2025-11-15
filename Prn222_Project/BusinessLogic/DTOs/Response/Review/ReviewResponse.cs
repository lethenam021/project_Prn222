using BusinessLogic.DTOs.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response.Review {
    public class ReviewResponse {
        public int Id { get; set; }
        public ProductResponse Product { get; set; }
        public string Reviewer { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
