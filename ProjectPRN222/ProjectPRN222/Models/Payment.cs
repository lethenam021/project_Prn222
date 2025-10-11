using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Payment")]
public partial class Payment
{
    [Key]
    public int id { get; set; }

    public int? orderId { get; set; }

    public int? userId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? amount { get; set; }

    [StringLength(50)]
    public string? method { get; set; }

    [StringLength(20)]
    public string? status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? paidAt { get; set; }

    [ForeignKey("orderId")]
    [InverseProperty("Payments")]
    public virtual OrderTable? order { get; set; }

    [ForeignKey("userId")]
    [InverseProperty("Payments")]
    public virtual User? user { get; set; }
}
