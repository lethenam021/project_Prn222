using System;

namespace BusinessLogic.DTOs.Response.Order
{
    
    public class SellerOrderDto
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }

        public string? BuyerName { get; set; }

        public string? ShippingAddress { get; set; }
    }
}