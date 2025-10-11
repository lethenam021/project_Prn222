using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    public int id { get; set; }

    public int? sellerId { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? averageRating { get; set; }

    public int? totalReviews { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? positiveRate { get; set; }

    [ForeignKey("sellerId")]
    [InverseProperty("Feedbacks")]
    public virtual User? seller { get; set; }
}
