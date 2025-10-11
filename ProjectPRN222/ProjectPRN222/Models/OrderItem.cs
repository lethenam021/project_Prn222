using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("OrderItem")]
public partial class OrderItem
{
    [Key]
    public int id { get; set; }

    public int? orderId { get; set; }

    public int? productId { get; set; }

    public int? quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? unitPrice { get; set; }

    [ForeignKey("orderId")]
    [InverseProperty("OrderItems")]
    public virtual OrderTable? order { get; set; }

    [ForeignKey("productId")]
    [InverseProperty("OrderItems")]
    public virtual Product? product { get; set; }
}
