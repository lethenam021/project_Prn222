using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    public int id { get; set; }

    [StringLength(100)]
    public string? name { get; set; }

    [InverseProperty("category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
