using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Dispute")]
public partial class Dispute
{
    [Key]
    public int id { get; set; }

    public int? orderId { get; set; }

    public int? raisedBy { get; set; }

    public string? description { get; set; }

    [StringLength(20)]
    public string? status { get; set; }

    public string? resolution { get; set; }

    [ForeignKey("orderId")]
    [InverseProperty("Disputes")]
    public virtual OrderTable? order { get; set; }

    [ForeignKey("raisedBy")]
    [InverseProperty("Disputes")]
    public virtual User? raisedByNavigation { get; set; }
}
