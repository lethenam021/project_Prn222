using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("ReturnRequest")]
public partial class ReturnRequest
{
    [Key]
    public int id { get; set; }

    public int? orderId { get; set; }

    public int? userId { get; set; }

    public string? reason { get; set; }

    [StringLength(20)]
    public string? status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? createdAt { get; set; }

    [ForeignKey("orderId")]
    [InverseProperty("ReturnRequests")]
    public virtual OrderTable? order { get; set; }

    [ForeignKey("userId")]
    [InverseProperty("ReturnRequests")]
    public virtual User? user { get; set; }
}
