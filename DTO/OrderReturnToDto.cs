using TazaFood_Core.Models.Order_Aggregate;

namespace TazaFood_API.DTO
{
    public class OrderReturnToDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; }
        public string OrderStaus { get; set; }

        public Address ShippingAddress { get; set; }

        public string DeliveryMethod { get; set; }
        public decimal Cost { get; set; }
        public string DeliveryTime { get; set; }

        public ICollection<OrderItemDto> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string PaymentIntent { get; set; }
    }
}
