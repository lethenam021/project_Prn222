using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("ShippingInfo")]
public partial class ShippingInfo
{
    [Key]
    public int id { get; set; }

    public int? orderId { get; set; }

    [StringLength(100)]
    public string? carrier { get; set; }

    [StringLength(100)]
    public string? trackingNumber { get; set; }

    [StringLength(50)]
    public string? status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? estimatedArrival { get; set; }

    [ForeignKey("orderId")]
    [InverseProperty("ShippingInfos")]
    public virtual OrderTable? order { get; set; }
}
