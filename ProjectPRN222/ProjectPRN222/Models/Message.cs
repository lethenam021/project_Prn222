using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

[Table("Message")]
public partial class Message
{
    [Key]
    public int id { get; set; }

    public int? senderId { get; set; }

    public int? receiverId { get; set; }

    public string? content { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? timestamp { get; set; }

    [ForeignKey("receiverId")]
    [InverseProperty("Messagereceivers")]
    public virtual User? receiver { get; set; }

    [ForeignKey("senderId")]
    [InverseProperty("Messagesenders")]
    public virtual User? sender { get; set; }
}
