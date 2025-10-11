using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Address")]
public partial class Address
{
    [Key]
    public int id { get; set; }

    public int? userId { get; set; }

    [StringLength(100)]
    public string? fullName { get; set; }

    [StringLength(20)]
    public string? phone { get; set; }

    [StringLength(100)]
    public string? street { get; set; }

    [StringLength(50)]
    public string? city { get; set; }

    [StringLength(50)]
    public string? state { get; set; }

    [StringLength(50)]
    public string? country { get; set; }

    public bool? isDefault { get; set; }

    [InverseProperty("address")]
    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    [ForeignKey("userId")]
    [InverseProperty("Addresses")]
    public virtual User? user { get; set; }
}
