using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("User")]
[Index("email", Name = "UQ__User__AB6E616432AE1EAF", IsUnique = true)]
public partial class User
{
    [Key]
    public int id { get; set; }

    [StringLength(100)]
    public string? username { get; set; }

    [StringLength(100)]
    public string? email { get; set; }

    [StringLength(255)]
    public string? password { get; set; }

    [StringLength(20)]
    public string? role { get; set; }

    public string? avatarURL { get; set; }

    [InverseProperty("user")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [InverseProperty("bidder")]
    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    [InverseProperty("raisedByNavigation")]
    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    [InverseProperty("seller")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("receiver")]
    public virtual ICollection<Message> Messagereceivers { get; set; } = new List<Message>();

    [InverseProperty("sender")]
    public virtual ICollection<Message> Messagesenders { get; set; } = new List<Message>();

    [InverseProperty("buyer")]
    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    [InverseProperty("user")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("seller")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("user")]
    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();

    [InverseProperty("reviewer")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("seller")]
    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
