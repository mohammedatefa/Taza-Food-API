namespace TazaFood_API.DTO
{
    public class OrderDto
    {
        public string  cartId { get; set; }
        public int DeliveryMethod { get; set; }
        public UserAddressDto ShippingAddress { get; set; }
    }
}
