using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    public string? title { get; set; }

    public string? description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? price { get; set; }

    public string? images { get; set; }

    public int? categoryId { get; set; }

    public int? sellerId { get; set; }

    public bool? isAuction { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? auctionEndTime { get; set; }

    [InverseProperty("product")]
    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    [InverseProperty("product")]
    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    [InverseProperty("product")]
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    [InverseProperty("product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("product")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("categoryId")]
    [InverseProperty("Products")]
    public virtual Category? category { get; set; }

    [ForeignKey("sellerId")]
    [InverseProperty("Products")]
    public virtual User? seller { get; set; }
}
