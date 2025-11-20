using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ReviewReply
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public int SellerId { get; set; }

    public string ReplyMessage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Review Review { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;
}
