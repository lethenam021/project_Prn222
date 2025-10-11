using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Inventory")]
public partial class Inventory
{
    [Key]
    public int id { get; set; }

    public int? productId { get; set; }

    public int? quantity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? lastUpdated { get; set; }

    [ForeignKey("productId")]
    [InverseProperty("Inventories")]
    public virtual Product? product { get; set; }
}
