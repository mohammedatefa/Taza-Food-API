using AutoMapper;
using TazaFood_API.DTO;
using TazaFood_Core.Models;
using TazaFood_Core.Models.Order_Aggregate;

namespace TazaFood_API.Helpers
{
    public class OrderItemImageResolve : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration configration;

        public OrderItemImageResolve(IConfiguration configration)
        {
            this.configration = configration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.ImageUrl))
            {
                return $"{configration["ApiBaseUrl"]}/{source.Product.ImageUrl}";
            }
            else
                return string.Empty;
        }
    }
}
