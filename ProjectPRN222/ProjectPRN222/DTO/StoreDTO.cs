using ProjectPRN222.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectPRN222.DTO
{
    public class StoreDTO
    {
        [Key]
        public int id { get; set; }

        public int? sellerId { get; set; }

        [StringLength(100)]
        public string? storeName { get; set; }

        public string? description { get; set; }

        public string? bannerImageURL { get; set; }

        public virtual User? seller { get; set; }
    }
}
