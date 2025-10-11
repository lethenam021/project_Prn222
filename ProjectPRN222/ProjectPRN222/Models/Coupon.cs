using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Coupon")]
public partial class Coupon
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    public string? code { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? discountPercent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? startDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? endDate { get; set; }

    public int? maxUsage { get; set; }

    public int? productId { get; set; }

    [ForeignKey("productId")]
    [InverseProperty("Coupons")]
    public virtual Product? product { get; set; }
}
