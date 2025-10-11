using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Store")]
public partial class Store
{
    [Key]
    public int id { get; set; }

    public int? sellerId { get; set; }

    [StringLength(100)]
    public string? storeName { get; set; }

    public string? description { get; set; }

    public string? bannerImageURL { get; set; }

    [ForeignKey("sellerId")]
    [InverseProperty("Stores")]
    public virtual User? seller { get; set; }
}
