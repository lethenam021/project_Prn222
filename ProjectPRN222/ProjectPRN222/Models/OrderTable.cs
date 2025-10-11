using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("OrderTable")]
public partial class OrderTable
{
    [Key]
    public int id { get; set; }

    public int? buyerId { get; set; }

    public int? addressId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? orderDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? totalPrice { get; set; }

    [StringLength(20)]
    public string? status { get; set; }

    [InverseProperty("order")]
    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    [InverseProperty("order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("order")]
    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();

    [InverseProperty("order")]
    public virtual ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();

    [ForeignKey("addressId")]
    [InverseProperty("OrderTables")]
    public virtual Address? address { get; set; }

    [ForeignKey("buyerId")]
    [InverseProperty("OrderTables")]
    public virtual User? buyer { get; set; }
}
