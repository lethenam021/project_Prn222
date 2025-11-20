using System;

namespace BusinessLogic.DTOs.Response.Order
{
    
    public class ShippingLabelDto
    {
        public int OrderId { get; set; }
        public string? Carrier { get; set; } 
        public string? TrackingNumber { get; set; } 
        public DateTime? EstimatedArrival { get; set; }

        public string? BuyerName { get; set; }
        public string? FullAddress { get; set; }
        public string? Phone { get; set; }
    }
}