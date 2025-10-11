using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Bid")]
public partial class Bid
{
    [Key]
    public int id { get; set; }

    public int? productId { get; set; }

    public int? bidderId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? bidTime { get; set; }

    [ForeignKey("bidderId")]
    [InverseProperty("Bids")]
    public virtual User? bidder { get; set; }

    [ForeignKey("productId")]
    [InverseProperty("Bids")]
    public virtual Product? product { get; set; }
}
