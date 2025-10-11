using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Review")]
public partial class Review
{
    [Key]
    public int id { get; set; }

    public int? productId { get; set; }

    public int? reviewerId { get; set; }

    public int? rating { get; set; }

    public string? comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? createdAt { get; set; }

    [ForeignKey("productId")]
    [InverseProperty("Reviews")]
    public virtual Product? product { get; set; }

    [ForeignKey("reviewerId")]
    [InverseProperty("Reviews")]
    public virtual User? reviewer { get; set; }
}
